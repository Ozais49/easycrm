using Cem.Identity.Application.Abstractions;
using Cem.Identity.Application.Members;
using Cem.Identity.Application.Me;
using Cem.Identity.Application.Permissions;
using Cem.Identity.Application.Roles;
using Cem.Identity.Application.Teams;
using Cem.Identity.Domain;
using Cem.Infrastructure.Common.Persistence;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Cem.Identity.Infrastructure.Persistence;

public sealed class IdentityQueries(IdentityDbContext db) : IIdentityQueries
{
    public async Task<PermissionSnapshot> GetPermissionSnapshotAsync(Guid userId, CancellationToken cancellationToken)
    {
        var matrices = await (
            from membership in db.TenantMemberships.AsNoTracking()
            join assignment in db.MembershipRoles on membership.Id equals assignment.MembershipId
            join role in db.Roles on assignment.RoleId equals role.Id
            where membership.UserId == userId && membership.Status == MembershipStatus.Active
            select role.Permissions).ToListAsync(cancellationToken);

        // Soft-deleted teams drop out through the Teams query filter.
        var teamIds = await (
            from membership in db.TenantMemberships.AsNoTracking()
            join teamMember in db.TeamMembers on membership.Id equals teamMember.MembershipId
            join team in db.Teams on teamMember.TeamId equals team.Id
            where membership.UserId == userId && membership.Status == MembershipStatus.Active
            select team.Id).ToListAsync(cancellationToken);

        return new PermissionSnapshot(PermissionMatrix.Union(matrices), teamIds);
    }

    public async Task<MeIdentity?> GetMeAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken)
    {
        var user = await db.Users.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new MeUserDto(u.Id, u.Email, u.FirstName, u.LastName, u.ImageUrl))
            .FirstOrDefaultAsync(cancellationToken);

        var tenant = await db.Tenants.AsNoTracking()
            .Where(t => t.Id == tenantId)
            .Select(t => new MeTenantDto(t.Id, t.Name, t.Slug))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null || tenant is null)
        {
            return null;
        }

        var roles = await (
            from membership in db.TenantMemberships.AsNoTracking()
            join assignment in db.MembershipRoles on membership.Id equals assignment.MembershipId
            join role in db.Roles on assignment.RoleId equals role.Id
            where membership.UserId == userId
            orderby role.Name
            select role.Name).ToListAsync(cancellationToken);

        return new MeIdentity(user, tenant, roles);
    }

    public Task<PagedResult<MemberDto>> ListMembersAsync(int page, int pageSize, CancellationToken cancellationToken) =>
        (from membership in db.TenantMemberships.AsNoTracking()
         join user in db.Users on membership.UserId equals user.Id
         orderby membership.JoinedAt, membership.Id
         select new MemberDto(
             membership.Id,
             user.Id,
             user.Email,
             user.FirstName,
             user.LastName,
             membership.Status,
             db.MembershipRoles.Where(a => a.MembershipId == membership.Id).Select(a => a.RoleId).ToList()))
        .ToPagedAsync(page, pageSize, cancellationToken);

    public Task<PagedResult<RoleDto>> ListRolesAsync(int page, int pageSize, CancellationToken cancellationToken) =>
        db.Roles.AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto(r.Id, r.Name, r.IsSystem, r.Permissions))
            .ToPagedAsync(page, pageSize, cancellationToken);

    public Task<RoleDto?> GetRoleAsync(Guid roleId, CancellationToken cancellationToken) =>
        db.Roles.AsNoTracking()
            .Where(r => r.Id == roleId)
            .Select(r => new RoleDto(r.Id, r.Name, r.IsSystem, r.Permissions))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<PagedResult<TeamDto>> ListTeamsAsync(int page, int pageSize, CancellationToken cancellationToken) =>
        db.Teams.AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TeamDto(
                t.Id,
                t.Name,
                db.TeamMembers.Where(m => m.TeamId == t.Id).Select(m => m.MembershipId).ToList()))
            .ToPagedAsync(page, pageSize, cancellationToken);
}
