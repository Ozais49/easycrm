using Cem.Identity.Domain;
using Cem.SharedKernel;

namespace Cem.Identity.Application.Abstractions;

/// <summary>
/// Finds or creates the tenant, user and membership behind a Clerk identity. Used both by just-in-time
/// provisioning from session tokens and by webhooks, which may arrive in any order. Safe under concurrency:
/// losing an insert race falls back to loading the winner row. A tenant always ends up with the seeded roles
/// and an active member with a role.
/// </summary>
public sealed class IdentityProvisioner(IIdentityStore store, IRequestContextSetter requestContext, TimeProvider timeProvider)
{
    /// <summary>Null when the tenant exists but was soft-deleted.</summary>
    public Task<Tenant?> EnsureTenantAsync(string clerkOrgId, string? slug, CancellationToken cancellationToken) =>
        EnsureAsync(
            () => store.FindTenantAsync(clerkOrgId, cancellationToken),
            // Name and slug are placeholders until the organization webhook delivers the real ones.
            () => Tenant.Create(clerkOrgId, name: slug ?? clerkOrgId, slug: slug ?? clerkOrgId),
            tenant =>
            {
                requestContext.SetTenant(tenant.Id);
                return SeedRolesAsync(cancellationToken);
            },
            cancellationToken);

    /// <summary>Null when the user exists but was soft-deleted.</summary>
    public Task<User?> EnsureUserAsync(string clerkUserId, CancellationToken cancellationToken) =>
        EnsureAsync(
            () => store.FindUserAsync(clerkUserId, cancellationToken),
            () => User.Create(clerkUserId),
            afterCreated: null,
            cancellationToken);

    /// <summary>
    /// Switches the scope to <paramref name="tenant"/>, then finds or creates the membership of the user in it.
    /// A member without any role gets Admin when the Clerk organization role is admin, otherwise Viewer. Members
    /// with roles keep them, since admins may have changed them (and cannot remove the last one).
    /// </summary>
    public async Task<TenantMembership> EnsureMembershipAsync(
        Tenant tenant,
        User user,
        bool isOrgAdmin,
        CancellationToken cancellationToken)
    {
        requestContext.SetTenant(tenant.Id);

        // Self-healing: the steps below are separate saves, so a crash can leave a tenant without roles or a member without one.
        await SeedRolesAsync(cancellationToken);

        var membership = await EnsureAsync(
            () => store.FindMembershipAsync(user.Id, cancellationToken),
            () => TenantMembership.Create(user.Id, timeProvider.GetUtcNow()),
            afterCreated: null,
            cancellationToken)
            ?? throw new InvalidOperationException("Membership could not be created or loaded.");

        if (membership.IsActive)
        {
            await AssignDefaultRoleIfNoneAsync(membership, isOrgAdmin, cancellationToken);
        }

        return membership;
    }

    /// <summary>Seeds the default roles of the current tenant unless it has them. Needs the tenant set (stamping and RLS).</summary>
    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        if (await store.FindAdminRoleAsync(cancellationToken) is not null)
        {
            return;
        }

        foreach (var role in SeedRoles.CreateAll())
        {
            store.Add(role);
        }

        // Losing to a concurrent seeding run (unique tenant + name) is fine: the roles exist either way.
        await store.TrySaveNewAsync(cancellationToken);
    }

    private async Task AssignDefaultRoleIfNoneAsync(TenantMembership membership, bool isOrgAdmin, CancellationToken cancellationToken)
    {
        if (await store.MembershipHasRolesAsync(membership.Id, cancellationToken))
        {
            return;
        }

        var role = isOrgAdmin
            ? await store.FindAdminRoleAsync(cancellationToken)
            : await store.FindRoleByNameAsync(SeedRoles.ViewerName, cancellationToken);

        // No Viewer role (an admin deleted it): the member has none until an admin assigns one.
        if (role is null)
        {
            return;
        }

        store.Add(MembershipRole.Create(membership.Id, role.Id));
        await store.TrySaveNewAsync(cancellationToken);
    }

    private async Task<T?> EnsureAsync<T>(
        Func<Task<T?>> find,
        Func<T> create,
        Func<T, Task>? afterCreated,
        CancellationToken cancellationToken)
        where T : Entity
    {
        var existing = await find();
        if (existing is not null)
        {
            return existing;
        }

        var created = create();
        store.Add(created);

        if (!await store.TrySaveNewAsync(cancellationToken))
        {
            return await find();
        }

        if (afterCreated is not null)
        {
            await afterCreated(created);
        }

        return created;
    }
}
