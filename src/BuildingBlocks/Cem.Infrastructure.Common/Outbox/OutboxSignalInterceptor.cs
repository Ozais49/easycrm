using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cem.Infrastructure.Common.Outbox;

/// <summary>
/// Notifies <see cref="OutboxSignal"/> after a SaveChanges that inserted outbox messages has committed.
/// Inside an explicit transaction the commit comes later, so no notification is sent; the dispatcher's
/// fallback poll picks those messages up.
/// </summary>
public sealed class OutboxSignalInterceptor(OutboxSignal signal) : SaveChangesInterceptor
{
    private bool _hasNewMessages;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _hasNewMessages = eventData.Context?.ChangeTracker.Entries<OutboxMessage>()
            .Any(e => e.State == EntityState.Added) ?? false;
        return ValueTask.FromResult(result);
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (_hasNewMessages && eventData.Context?.Database.CurrentTransaction is null)
        {
            signal.Notify();
        }

        _hasNewMessages = false;
        return ValueTask.FromResult(result);
    }
}
