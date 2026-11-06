using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Teams;

public sealed class ListTeamsHandler(IIdentityQueries queries)
    : IQueryHandler<ListTeamsQuery, Result<PagedResult<TeamDto>>>
{
    public async ValueTask<Result<PagedResult<TeamDto>>> Handle(ListTeamsQuery query, CancellationToken cancellationToken) =>
        await queries.ListTeamsAsync(query.Page, query.PageSize, cancellationToken);
}
