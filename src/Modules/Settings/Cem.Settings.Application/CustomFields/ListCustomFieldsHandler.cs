using Cem.Settings.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.CustomFields;

public sealed class ListCustomFieldsHandler(ICustomFieldStore store)
    : IQueryHandler<ListCustomFieldsQuery, Result<IReadOnlyList<CustomFieldDto>>>
{
    public async ValueTask<Result<IReadOnlyList<CustomFieldDto>>> Handle(
        ListCustomFieldsQuery query, CancellationToken cancellationToken)
    {
        var definitions = await store.ListActiveReadOnlyAsync(query.EntityType, cancellationToken);
        return Result<IReadOnlyList<CustomFieldDto>>.Success(definitions.Select(CustomFieldDto.From).ToList());
    }
}
