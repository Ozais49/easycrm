namespace Cem.Infrastructure.Common.Outbox;

/// <summary>
/// Wakes the outbox dispatcher when a SaveChanges has written outbox messages, so it does not have to poll.
/// Several notifications before the dispatcher wakes collapse into one.
/// </summary>
public sealed class OutboxSignal
{
    private readonly SemaphoreSlim _signal = new(0, 1);

    public void Notify()
    {
        try
        {
            _signal.Release();
        }
        catch (SemaphoreFullException)
        {
            // Already signalled; the dispatcher will pick up every due message in its next round.
        }
    }

    /// <returns>True if signalled, false when the timeout passed first.</returns>
    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken) =>
        _signal.WaitAsync(timeout, cancellationToken);
}
