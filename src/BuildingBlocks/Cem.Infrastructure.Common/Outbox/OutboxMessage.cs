using Cem.SharedKernel;

namespace Cem.Infrastructure.Common.Outbox;

/// <summary>
/// An integration event waiting for delivery. Written by a module in the same transaction as its state change and
/// delivered by the outbox dispatcher (ADR-0005, ADR-0017).
/// </summary>
/// <remarks>
/// Deliberately not an <see cref="ITenantEntity"/> and without RLS: the dispatcher reads the messages of all tenants
/// and then acts for one tenant at a time. <see cref="TenantId"/> is only the tenant to act for; it is null when the
/// event happens before a tenant context exists (e.g. a tenant being created).
/// </remarks>
public sealed class OutboxMessage : Entity
{
    public const int MaxAttempts = 10;

    private const double MaxBackoffSeconds = 3600;
    private const int MaxErrorLength = 500;

    private OutboxMessage()
    {
    }

    public Guid? TenantId { get; private set; }
    public string EventType { get; private set; } = "";
    public string Payload { get; private set; } = "";
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public int Attempts { get; private set; }
    public string? LastError { get; private set; }
    public DateTimeOffset NextAttemptAt { get; private set; }

    /// <summary>Set after <see cref="MaxAttempts"/> failures: the message is kept for inspection and never retried.</summary>
    public DateTimeOffset? DeadAt { get; private set; }

    public bool IsDead => DeadAt is not null;

    public static OutboxMessage Create(Guid? tenantId, string eventType, string payload, DateTimeOffset now) =>
        new() { TenantId = tenantId, EventType = eventType, Payload = payload, OccurredAt = now, NextAttemptAt = now };

    public void MarkProcessed(DateTimeOffset now) => ProcessedAt = now;

    /// <summary>Counts the failure and schedules the next attempt in 2^attempts seconds (at most one hour), or gives up.</summary>
    public void RecordFailure(string error, DateTimeOffset now)
    {
        Attempts++;
        LastError = error.Length > MaxErrorLength ? error[..MaxErrorLength] : error;

        if (Attempts >= MaxAttempts)
        {
            DeadAt = now;
            return;
        }

        NextAttemptAt = now.AddSeconds(Math.Min(Math.Pow(2, Attempts), MaxBackoffSeconds));
    }
}
