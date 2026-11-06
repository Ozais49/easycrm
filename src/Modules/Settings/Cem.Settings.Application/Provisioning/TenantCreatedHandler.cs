using Cem.Identity.Contracts;
using Cem.Settings.Application.Abstractions;
using Cem.Settings.Domain;
using Cem.SharedKernel;

namespace Cem.Settings.Application.Provisioning;

/// <summary>Gives every new tenant its default settings. Idempotent: delivery is at-least-once.</summary>
public sealed class TenantCreatedHandler(ISettingsStore store, IRequestContextSetter requestContext)
    : IIntegrationEventHandler<TenantCreated>
{
    public async ValueTask HandleAsync(TenantCreated integrationEvent, CancellationToken cancellationToken)
    {
        // The event is published before any tenant context exists, so it carries none; take it from the payload.
        requestContext.SetTenant(integrationEvent.TenantId);

        if (await store.FindReadOnlyAsync(cancellationToken) is not null)
        {
            return;
        }

        store.Add(TenantSettings.CreateDefault());

        // False means a concurrent request created the row first, which is just as good.
        await store.TrySaveNewAsync(cancellationToken);
    }
}
