using Cem.SharedKernel;

namespace Cem.Infrastructure.Common.Tenancy;

/// <summary>
/// Scoped holder of the current tenant and user. A scope can only ever belong to one tenant and one user,
/// so a bug cannot silently carry data across tenants within a request.
/// </summary>
public sealed class RequestContext : ITenantContext, ICurrentUser, IRequestContextSetter
{
    public Guid? TenantId { get; private set; }

    public bool HasTenant => TenantId is not null;

    public Guid? UserId { get; private set; }

    public string? ClerkUserId { get; private set; }

    public void SetTenant(Guid tenantId)
    {
        if (TenantId is { } current && current != tenantId)
        {
            throw new InvalidOperationException("This scope already belongs to a different tenant.");
        }

        TenantId = tenantId;
    }

    public void SetUser(Guid userId, string clerkUserId)
    {
        if (UserId is { } current && current != userId)
        {
            throw new InvalidOperationException("This scope already belongs to a different user.");
        }

        UserId = userId;
        ClerkUserId = clerkUserId;
    }
}
