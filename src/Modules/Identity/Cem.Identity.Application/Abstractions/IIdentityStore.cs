using Cem.Identity.Domain;
using Cem.SharedKernel;

namespace Cem.Identity.Application.Abstractions;

/// <summary>Persistence port of the Identity module. Memberships are looked up within the current tenant.</summary>
public interface IIdentityStore
{
    Task<Tenant?> FindTenantAsync(string clerkOrgId, CancellationToken cancellationToken);

    Task<User?> FindUserAsync(string clerkUserId, CancellationToken cancellationToken);

    Task<TenantMembership?> FindMembershipAsync(Guid userId, CancellationToken cancellationToken);

    void Add(Entity entity);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Saves, returning false (and dropping the pending inserts) when a unique constraint rejected them
    /// because a concurrent request created the same row first.
    /// </summary>
    Task<bool> TrySaveNewAsync(CancellationToken cancellationToken);
}
