using Cem.SharedKernel;

namespace Cem.Identity.Contracts;

/// <summary>A user got a new membership in a tenant (not published when a removed member re-joins).</summary>
public sealed record UserJoined(Guid TenantId, Guid UserId, Guid MembershipId) : IIntegrationEvent
{
    public static string EventType => "identity.user_joined";
}
