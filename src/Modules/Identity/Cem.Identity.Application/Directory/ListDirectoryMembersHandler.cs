using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Directory;

public sealed class ListDirectoryMembersHandler(IIdentityQueries queries)
    : IQueryHandler<ListDirectoryMembersQuery, Result<IReadOnlyList<DirectoryMemberDto>>>
{
    public async ValueTask<Result<IReadOnlyList<DirectoryMemberDto>>> Handle(
        ListDirectoryMembersQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyList<DirectoryMemberDto>>.Success(await queries.ListDirectoryMembersAsync(cancellationToken));
}
