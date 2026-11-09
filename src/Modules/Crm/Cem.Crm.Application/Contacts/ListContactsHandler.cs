using Cem.Crm.Application.Abstractions;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Contacts;

public sealed class ListContactsHandler(ICrmQueries queries, IPermissionService permissions, ICurrentUser user)
    : IQueryHandler<ListContactsQuery, Result<PagedResult<ContactDto>>>
{
    public async ValueTask<Result<PagedResult<ContactDto>>> Handle(ListContactsQuery query, CancellationToken cancellationToken) =>
        await queries.ListContactsAsync(
            RecordAccess.For(Scope.Crm, PermissionAction.Read, permissions, user),
            Text.Clean(query.Search),
            query.CompanyId,
            query.TagId,
            query.Page,
            query.PageSize,
            cancellationToken);
}
