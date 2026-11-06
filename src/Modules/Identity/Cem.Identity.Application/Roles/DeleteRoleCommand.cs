using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Roles;

public sealed record DeleteRoleCommand(Guid RoleId) : ICommand<Result>;
