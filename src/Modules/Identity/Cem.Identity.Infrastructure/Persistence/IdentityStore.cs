using Cem.Identity.Application.Abstractions;
using Cem.Identity.Domain;
using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Cem.Identity.Infrastructure.Persistence;

public sealed class IdentityStore(IdentityDbContext db) : IIdentityStore
{
    public Task<Tenant?> FindTenantAsync(string clerkOrgId, CancellationToken cancellationToken) =>
        db.Tenants.FirstOrDefaultAsync(t => t.ClerkOrgId == clerkOrgId, cancellationToken);

    public Task<User?> FindUserAsync(string clerkUserId, CancellationToken cancellationToken) =>
        db.Users.FirstOrDefaultAsync(u => u.ClerkUserId == clerkUserId, cancellationToken);

    public Task<TenantMembership?> FindMembershipAsync(Guid userId, CancellationToken cancellationToken) =>
        db.TenantMemberships.FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);

    public Task<TenantMembership?> FindMembershipByIdAsync(Guid membershipId, CancellationToken cancellationToken) =>
        db.TenantMemberships.FirstOrDefaultAsync(m => m.Id == membershipId, cancellationToken);

    public Task<int> CountActiveMembershipsAsync(IReadOnlyCollection<Guid> membershipIds, CancellationToken cancellationToken) =>
        db.TenantMemberships.CountAsync(m => membershipIds.Contains(m.Id) && m.Status == MembershipStatus.Active, cancellationToken);

    public Task<Role?> FindRoleAsync(Guid roleId, CancellationToken cancellationToken) =>
        db.Roles.FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

    public Task<Role?> FindRoleByNameAsync(string name, CancellationToken cancellationToken) =>
        db.Roles.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);

    public Task<Role?> FindAdminRoleAsync(CancellationToken cancellationToken) =>
        db.Roles.FirstOrDefaultAsync(r => r.IsSystem, cancellationToken);

    public Task<bool> RoleNameTakenAsync(string name, Guid? exceptRoleId, CancellationToken cancellationToken) =>
        db.Roles.AnyAsync(r => r.Name == name && r.Id != exceptRoleId, cancellationToken);

    public Task<bool> RoleIsAssignedAsync(Guid roleId, CancellationToken cancellationToken) =>
        db.MembershipRoles.AnyAsync(m => m.RoleId == roleId, cancellationToken);

    public Task<bool> MembershipHasRolesAsync(Guid membershipId, CancellationToken cancellationToken) =>
        db.MembershipRoles.AnyAsync(m => m.MembershipId == membershipId, cancellationToken);

    public Task<int> CountRolesAsync(IReadOnlyCollection<Guid> roleIds, CancellationToken cancellationToken) =>
        db.Roles.CountAsync(r => roleIds.Contains(r.Id), cancellationToken);

    public Task<List<MembershipRole>> ListMembershipRolesAsync(Guid membershipId, CancellationToken cancellationToken) =>
        db.MembershipRoles.Where(m => m.MembershipId == membershipId).ToListAsync(cancellationToken);

    public Task<bool> OtherActiveAdminExistsAsync(Guid membershipId, CancellationToken cancellationToken) =>
        (from assignment in db.MembershipRoles
         join role in db.Roles on assignment.RoleId equals role.Id
         join membership in db.TenantMemberships on assignment.MembershipId equals membership.Id
         where role.IsSystem && membership.Status == MembershipStatus.Active && membership.Id != membershipId
         select membership.Id).AnyAsync(cancellationToken);

    public Task<Team?> FindTeamAsync(Guid teamId, CancellationToken cancellationToken) =>
        db.Teams.FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);

    public Task<bool> TeamNameTakenAsync(string name, Guid? exceptTeamId, CancellationToken cancellationToken) =>
        db.Teams.AnyAsync(t => t.Name == name && t.Id != exceptTeamId, cancellationToken);

    public Task<List<TeamMember>> ListTeamMembersAsync(Guid teamId, CancellationToken cancellationToken) =>
        db.TeamMembers.Where(m => m.TeamId == teamId).ToListAsync(cancellationToken);

    public void Add(Entity entity) => db.Add(entity);

    public void Remove(Entity entity) => db.Remove(entity);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);

    public async Task<bool> TrySaveNewAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Drop only the rejected inserts so entities already loaded in this scope stay tracked.
            foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList())
            {
                entry.State = EntityState.Detached;
            }

            return false;
        }
    }
}
