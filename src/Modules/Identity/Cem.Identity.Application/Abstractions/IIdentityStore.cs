using Cem.Identity.Domain;
using Cem.SharedKernel;

namespace Cem.Identity.Application.Abstractions;

/// <summary>Persistence port of the Identity module. Memberships are looked up within the current tenant.</summary>
public interface IIdentityStore
{
    Task<Tenant?> FindTenantAsync(string clerkOrgId, CancellationToken cancellationToken);

    Task<User?> FindUserAsync(string clerkUserId, CancellationToken cancellationToken);

    Task<TenantMembership?> FindMembershipAsync(Guid userId, CancellationToken cancellationToken);

    Task<TenantMembership?> FindMembershipByIdAsync(Guid membershipId, CancellationToken cancellationToken);

    /// <summary>How many of the ids are active memberships of the current tenant.</summary>
    Task<int> CountActiveMembershipsAsync(IReadOnlyCollection<Guid> membershipIds, CancellationToken cancellationToken);

    Task<Role?> FindRoleAsync(Guid roleId, CancellationToken cancellationToken);

    Task<Role?> FindRoleByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>The tenant's system role (Admin), if it still exists.</summary>
    Task<Role?> FindAdminRoleAsync(CancellationToken cancellationToken);

    /// <summary>True when another role of the current tenant has this name.</summary>
    Task<bool> RoleNameTakenAsync(string name, Guid? exceptRoleId, CancellationToken cancellationToken);

    Task<bool> RoleIsAssignedAsync(Guid roleId, CancellationToken cancellationToken);

    Task<bool> MembershipHasRolesAsync(Guid membershipId, CancellationToken cancellationToken);

    /// <summary>How many of the ids are roles of the current tenant.</summary>
    Task<int> CountRolesAsync(IReadOnlyCollection<Guid> roleIds, CancellationToken cancellationToken);

    Task<List<MembershipRole>> ListMembershipRolesAsync(Guid membershipId, CancellationToken cancellationToken);

    /// <summary>True when an active membership other than the given one holds the system role.</summary>
    Task<bool> OtherActiveAdminExistsAsync(Guid membershipId, CancellationToken cancellationToken);

    Task<Team?> FindTeamAsync(Guid teamId, CancellationToken cancellationToken);

    Task<bool> TeamNameTakenAsync(string name, Guid? exceptTeamId, CancellationToken cancellationToken);

    Task<List<TeamMember>> ListTeamMembersAsync(Guid teamId, CancellationToken cancellationToken);

    void Add(Entity entity);

    void Remove(Entity entity);

    /// <summary>Queues an integration event, written by the next save in the same transaction as the other changes.</summary>
    void Publish<TEvent>(TEvent integrationEvent)
        where TEvent : IIntegrationEvent;

    Task SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Saves, returning false when a unique constraint rejected the changes because a concurrent request wrote the
    /// same row or name first. Pending inserts are dropped so entities already loaded in this scope stay tracked.
    /// </summary>
    Task<bool> TrySaveNewAsync(CancellationToken cancellationToken);
}
