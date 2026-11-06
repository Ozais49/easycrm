using Cem.SharedKernel;

namespace Cem.Identity.Contracts;

/// <summary>
/// A tenant row was created. The outbox message has no tenant context (none exists yet), so handlers that touch
/// tenant data must set the tenant from <see cref="TenantId"/> themselves.
/// </summary>
public sealed record TenantCreated(Guid TenantId, string Name) : IIntegrationEvent
{
    public static string EventType => "identity.tenant_created";
}
