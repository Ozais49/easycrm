namespace Cem.SharedKernel;

/// <summary>
/// Reacts to an integration event. Delivery is at-least-once (a crash after the handler ran but before the message
/// was marked processed, or a retry after a failure, runs it again), so handlers MUST be idempotent.
/// Runs in its own DI scope with the tenant of the message set (when it has one). Throwing schedules a retry.
/// </summary>
public interface IIntegrationEventHandler<TEvent>
    where TEvent : IIntegrationEvent
{
    ValueTask HandleAsync(TEvent integrationEvent, CancellationToken cancellationToken);
}
