using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Identity.Application.Roles;

public sealed record UpdateRoleCommand(Guid RoleId, string Name, PermissionMatrix Permissions) : ICommand<Result<RoleDto>>;
