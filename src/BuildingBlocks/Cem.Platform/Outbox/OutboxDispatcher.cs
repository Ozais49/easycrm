using Cem.Infrastructure.Common.Outbox;
using Cem.Platform.Persistence;
using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cem.Platform.Outbox;

/// <summary>
/// Delivers outbox messages in-process, one at a time (ADR-0005, ADR-0017). Each round locks a batch of due
/// messages with <c>FOR UPDATE SKIP LOCKED</c> in a transaction, runs the handlers of each in their own DI scope with
/// the tenant of the message, and records the outcome. Delivery is at-least-once: handlers must be idempotent.
/// </summary>
public sealed class OutboxDispatcher(
    IServiceScopeFactory scopeFactory,
    IEnumerable<IntegrationEventRegistration> registrations,
    OutboxSignal signal,
    TimeProvider timeProvider,
    ILogger<OutboxDispatcher> logger) : BackgroundService
{
    private const int BatchSize = 20;

    /// <summary>
    /// When idle, the dispatcher sleeps until <see cref="OutboxSignal"/> says new messages were saved. This poll is
    /// only the safety net for retries that come due and messages saved inside an explicit transaction.
    /// </summary>
    private static readonly TimeSpan FallbackPollInterval = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan FailureBaseDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan MaxFailureDelay = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan Retention = TimeSpan.FromDays(7);
    private static readonly TimeSpan PurgeInterval = TimeSpan.FromHours(1);

    private readonly Dictionary<string, IntegrationEventRegistration> _registrations =
        registrations.DistinctBy(r => r.EventType).ToDictionary(r => r.EventType);

    private DateTimeOffset _nextPurgeAt;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var failures = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            TimeSpan delay;

            try
            {
                var delivered = await DispatchBatchAsync(stoppingToken);
                await PurgeProcessedIfDueAsync(stoppingToken);

                failures = 0;
                if (delivered < BatchSize)
                {
                    await signal.WaitAsync(FallbackPollInterval, stoppingToken);
                }

                continue;
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                // Typically the database is unreachable. Back off (2 s doubling up to 1 min) instead of crashing the host.
                failures++;
                delay = TimeSpan.FromSeconds(Math.Min(FailureBaseDelay.TotalSeconds * Math.Pow(2, failures), MaxFailureDelay.TotalSeconds));
                logger.LogError("Outbox dispatch failed ({Error}); retrying in {Delay}.", ex.Message, delay);
            }

            try
            {
                await Task.Delay(delay, timeProvider, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    /// <returns>How many messages were handled; a full batch means there may be more.</returns>
    private async Task<int> DispatchBatchAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var now = timeProvider.GetUtcNow();
        var messages = await db.OutboxMessages
            .FromSql($"""
                SELECT * FROM platform.outbox_messages
                WHERE processed_at IS NULL AND dead_at IS NULL AND next_attempt_at <= {now}
                ORDER BY occurred_at
                LIMIT {BatchSize}
                FOR UPDATE SKIP LOCKED
                """)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            await DeliverAsync(message, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return messages.Count;
    }

    private async Task DeliverAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();

            if (message.TenantId is { } tenantId)
            {
                scope.ServiceProvider.GetRequiredService<IRequestContextSetter>().SetTenant(tenantId);
            }

            // No registration means nobody subscribed: the message is done.
            if (_registrations.TryGetValue(message.EventType, out var registration))
            {
                await registration.Deliver(scope.ServiceProvider, message.Payload, cancellationToken);
            }

            message.MarkProcessed(timeProvider.GetUtcNow());
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Type and message only: no stack trace in the table, and handlers must not put secrets in messages.
            message.RecordFailure($"{ex.GetType().Name}: {ex.Message}", timeProvider.GetUtcNow());

            logger.LogError(
                ex,
                "Outbox message {MessageId} ({EventType}) failed on attempt {Attempts}. {Outcome}",
                message.Id,
                message.EventType,
                message.Attempts,
                message.IsDead ? "Giving up; it stays in the table for inspection." : $"Next attempt at {message.NextAttemptAt:u}.");
        }
    }

    /// <summary>Deletes processed messages past retention, at most once per <see cref="PurgeInterval"/>. Dead messages stay.</summary>
    private async Task PurgeProcessedIfDueAsync(CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        if (now < _nextPurgeAt)
        {
            return;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();

        var cutoff = now - Retention;
        await db.OutboxMessages.Where(m => m.ProcessedAt < cutoff).ExecuteDeleteAsync(cancellationToken);

        _nextPurgeAt = now + PurgeInterval;
    }
}
