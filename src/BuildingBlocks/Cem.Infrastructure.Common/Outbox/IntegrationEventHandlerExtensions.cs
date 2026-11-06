using System.Text.Json;
using Cem.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace Cem.Infrastructure.Common.Outbox;

public static class IntegrationEventHandlerExtensions
{
    /// <summary>
    /// Subscribes <typeparamref name="THandler"/> to <typeparamref name="TEvent"/>. Several handlers may subscribe to
    /// the same event. Events nobody subscribed to are simply marked processed.
    /// </summary>
    public static IServiceCollection AddIntegrationEventHandler<TEvent, THandler>(this IServiceCollection services)
        where TEvent : IIntegrationEvent
        where THandler : class, IIntegrationEventHandler<TEvent>
    {
        services.AddScoped<IIntegrationEventHandler<TEvent>, THandler>();
        services.AddSingleton(new IntegrationEventRegistration(TEvent.EventType, DeliverAsync<TEvent>));
        return services;
    }

    private static async ValueTask DeliverAsync<TEvent>(IServiceProvider scope, string payload, CancellationToken cancellationToken)
        where TEvent : IIntegrationEvent
    {
        var integrationEvent = JsonSerializer.Deserialize<TEvent>(payload, IntegrationEventJson.Options)
            ?? throw new InvalidOperationException($"The payload of {TEvent.EventType} is empty.");

        foreach (var handler in scope.GetServices<IIntegrationEventHandler<TEvent>>())
        {
            await handler.HandleAsync(integrationEvent, cancellationToken);
        }
    }
}
