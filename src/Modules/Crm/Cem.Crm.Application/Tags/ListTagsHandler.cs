using Cem.Crm.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Tags;

public sealed class ListTagsHandler(ICrmQueries queries) : IQueryHandler<ListTagsQuery, Result<IReadOnlyList<TagDto>>>
{
    public async ValueTask<Result<IReadOnlyList<TagDto>>> Handle(ListTagsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyList<TagDto>>.Success(await queries.ListTagsAsync(cancellationToken));
}
