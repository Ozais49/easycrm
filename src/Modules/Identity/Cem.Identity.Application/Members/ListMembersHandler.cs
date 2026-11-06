using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Members;

public sealed class ListMembersHandler(IIdentityQueries queries)
    : IQueryHandler<ListMembersQuery, Result<PagedResult<MemberDto>>>
{
    public async ValueTask<Result<PagedResult<MemberDto>>> Handle(ListMembersQuery query, CancellationToken cancellationToken) =>
        await queries.ListMembersAsync(query.Page, query.PageSize, cancellationToken);
}
