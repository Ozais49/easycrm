using Cem.Crm.Application.Abstractions;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Companies;

public sealed class ListCompaniesHandler(ICrmQueries queries, IPermissionService permissions, ICurrentUser user)
    : IQueryHandler<ListCompaniesQuery, Result<PagedResult<CompanyDto>>>
{
    public async ValueTask<Result<PagedResult<CompanyDto>>> Handle(ListCompaniesQuery query, CancellationToken cancellationToken) =>
        await queries.ListCompaniesAsync(
            RecordAccess.For(Scope.Crm, PermissionAction.Read, permissions, user),
            Text.Clean(query.Search),
            query.TagId,
            query.Page,
            query.PageSize,
            cancellationToken);
}
