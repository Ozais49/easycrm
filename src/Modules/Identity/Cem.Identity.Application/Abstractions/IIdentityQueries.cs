using Cem.Identity.Application.Members;
using Cem.Identity.Application.Me;
using Cem.Identity.Application.Permissions;
using Cem.Identity.Application.Roles;
using Cem.Identity.Application.Teams;
using Cem.SharedKernel;

namespace Cem.Identity.Application.Abstractions;

/// <summary>Read side of the Identity module: no-tracking projections of the current tenant's data.</summary>
public interface IIdentityQueries
{
    /// <summary>Effective permissions and teams of an active member; empty permissions for anyone else.</summary>
    Task<PermissionSnapshot> GetPermissionSnapshotAsync(Guid userId, CancellationToken cancellationToken);

    Task<MeIdentity?> GetMeAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken);

    Task<PagedResult<MemberDto>> ListMembersAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<PagedResult<RoleDto>> ListRolesAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<RoleDto?> GetRoleAsync(Guid roleId, CancellationToken cancellationToken);

    Task<PagedResult<TeamDto>> ListTeamsAsync(int page, int pageSize, CancellationToken cancellationToken);
}
