namespace Cem.Infrastructure.Common.Outbox;

/// <summary>Maps a stored <see cref="Cem.SharedKernel.IIntegrationEvent.EventType"/> to the code that delivers it.</summary>
/// <param name="Deliver">Deserializes the payload and runs every registered handler, resolved from the given scope.</param>
public sealed record IntegrationEventRegistration(
    string EventType,
    Func<IServiceProvider, string, CancellationToken, ValueTask> Deliver);
