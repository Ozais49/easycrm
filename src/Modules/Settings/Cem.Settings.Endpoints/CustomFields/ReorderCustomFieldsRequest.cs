using Cem.Settings.Domain;

namespace Cem.Settings.Endpoints.CustomFields;

/// <summary>Body for reordering: the active fields of the entity type in their new order.</summary>
public sealed record ReorderCustomFieldsRequest(CustomFieldEntity EntityType, IReadOnlyList<Guid> Ids);
