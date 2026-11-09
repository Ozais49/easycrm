using Cem.Settings.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.CustomFields;

/// <param name="Ids">Active definitions of the entity type in the new order; any not listed follow in their current order.</param>
public sealed record ReorderCustomFieldsCommand(CustomFieldEntity EntityType, IReadOnlyList<Guid> Ids)
    : ICommand<Result<IReadOnlyList<CustomFieldDto>>>;
