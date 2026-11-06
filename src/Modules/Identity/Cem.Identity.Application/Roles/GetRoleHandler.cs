using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Roles;

public sealed class GetRoleHandler(IIdentityQueries queries) : IQueryHandler<GetRoleQuery, Result<RoleDto>>
{
    public async ValueTask<Result<RoleDto>> Handle(GetRoleQuery query, CancellationToken cancellationToken)
    {
        var role = await queries.GetRoleAsync(query.RoleId, cancellationToken);
        return role is null ? Result<RoleDto>.Failure(RoleErrors.NotFound) : role;
    }
}
