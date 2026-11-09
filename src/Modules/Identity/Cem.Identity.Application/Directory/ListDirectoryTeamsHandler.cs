using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Directory;

public sealed class ListDirectoryTeamsHandler(IIdentityQueries queries)
    : IQueryHandler<ListDirectoryTeamsQuery, Result<IReadOnlyList<DirectoryTeamDto>>>
{
    public async ValueTask<Result<IReadOnlyList<DirectoryTeamDto>>> Handle(
        ListDirectoryTeamsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyList<DirectoryTeamDto>>.Success(await queries.ListDirectoryTeamsAsync(cancellationToken));
}
