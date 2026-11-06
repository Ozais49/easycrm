using Cem.Identity.Domain;
using Cem.SharedKernel;

namespace Cem.Identity.Application.Abstractions;

/// <summary>
/// Finds or creates the tenant, user and membership behind a Clerk identity. Used both by just-in-time
/// provisioning from session tokens and by webhooks, which may arrive in any order. Safe under concurrency:
/// losing an insert race falls back to loading the winner's row.
/// </summary>
public sealed class IdentityProvisioner(IIdentityStore store, IRequestContextSetter requestContext, TimeProvider timeProvider)
{
    /// <summary>Null when the tenant exists but was soft-deleted.</summary>
    public Task<Tenant?> EnsureTenantAsync(string clerkOrgId, string? slug, CancellationToken cancellationToken) =>
        EnsureAsync(
            () => store.FindTenantAsync(clerkOrgId, cancellationToken),
            // Name and slug are placeholders until the organization webhook delivers the real ones.
            () => Tenant.Create(clerkOrgId, name: slug ?? clerkOrgId, slug: slug ?? clerkOrgId),
            cancellationToken);

    /// <summary>Null when the user exists but was soft-deleted.</summary>
    public Task<User?> EnsureUserAsync(string clerkUserId, CancellationToken cancellationToken) =>
        EnsureAsync(
            () => store.FindUserAsync(clerkUserId, cancellationToken),
            () => User.Create(clerkUserId),
            cancellationToken);

    /// <summary>Switches the scope to <paramref name="tenant"/>, then finds or creates the user's membership in it.</summary>
    public async Task<TenantMembership> EnsureMembershipAsync(Tenant tenant, User user, CancellationToken cancellationToken)
    {
        requestContext.SetTenant(tenant.Id);

        return await EnsureAsync(
            () => store.FindMembershipAsync(user.Id, cancellationToken),
            () => TenantMembership.Create(user.Id, timeProvider.GetUtcNow()),
            cancellationToken)
            ?? throw new InvalidOperationException("Membership could not be created or loaded.");
    }

    private async Task<T?> EnsureAsync<T>(Func<Task<T?>> find, Func<T> create, CancellationToken cancellationToken)
        where T : Entity
    {
        var existing = await find();
        if (existing is not null)
        {
            return existing;
        }

        var created = create();
        store.Add(created);

        return await store.TrySaveNewAsync(cancellationToken) ? created : await find();
    }
}
