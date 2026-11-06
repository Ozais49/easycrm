using Cem.SharedKernel.Permissions;

namespace Cem.Identity.Application.Me;

public sealed record MeDto(
    MeUserDto User,
    MeTenantDto Tenant,
    IReadOnlyList<string> Roles,
    IReadOnlyCollection<Guid> TeamIds,
    PermissionMatrix Permissions);
