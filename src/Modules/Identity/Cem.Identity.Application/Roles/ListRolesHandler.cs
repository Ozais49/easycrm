using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Roles;

public sealed class ListRolesHandler(IIdentityQueries queries)
    : IQueryHandler<ListRolesQuery, Result<PagedResult<RoleDto>>>
{
    public async ValueTask<Result<PagedResult<RoleDto>>> Handle(ListRolesQuery query, CancellationToken cancellationToken) =>
        await queries.ListRolesAsync(query.Page, query.PageSize, cancellationToken);
}
