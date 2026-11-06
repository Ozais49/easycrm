using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Identity.Application.Roles;

public sealed record CreateRoleCommand(string Name, PermissionMatrix Permissions) : ICommand<Result<RoleDto>>;
