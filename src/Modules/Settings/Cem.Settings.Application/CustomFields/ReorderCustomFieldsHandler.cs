using Cem.Settings.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.CustomFields;

public sealed class ReorderCustomFieldsHandler(ICustomFieldStore store)
    : ICommandHandler<ReorderCustomFieldsCommand, Result<IReadOnlyList<CustomFieldDto>>>
{
    public async ValueTask<Result<IReadOnlyList<CustomFieldDto>>> Handle(
        ReorderCustomFieldsCommand command, CancellationToken cancellationToken)
    {
        var active = await store.ListActiveAsync(command.EntityType, cancellationToken);
        var byId = active.ToDictionary(d => d.Id);

        if (command.Ids.Any(id => !byId.ContainsKey(id)))
        {
            return Result<IReadOnlyList<CustomFieldDto>>.Failure(
                CustomFieldErrors.Invalid("Ids", "Every id must be an active custom field of this entity type."));
        }

        var listed = command.Ids.Select(id => byId[id]);
        var ordered = listed.Concat(active.Where(d => !command.Ids.Contains(d.Id))).ToList();
        for (var i = 0; i < ordered.Count; i++)
        {
            if (ordered[i].SortOrder != i)
            {
                ordered[i].MoveTo(i);
            }
        }

        await store.SaveChangesAsync(cancellationToken);
        return Result<IReadOnlyList<CustomFieldDto>>.Success(ordered.Select(CustomFieldDto.From).ToList());
    }
}
