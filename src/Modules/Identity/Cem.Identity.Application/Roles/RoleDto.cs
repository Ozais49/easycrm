using Cem.SharedKernel.Permissions;

namespace Cem.Identity.Application.Roles;

public sealed record RoleDto(Guid Id, string Name, bool IsSystem, PermissionMatrix Permissions);
