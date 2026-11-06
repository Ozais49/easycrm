using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Roles;

public sealed record GetRoleQuery(Guid RoleId) : IQuery<Result<RoleDto>>;
