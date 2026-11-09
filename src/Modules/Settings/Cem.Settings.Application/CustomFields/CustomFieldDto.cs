using Cem.Settings.Domain;

namespace Cem.Settings.Application.CustomFields;

public sealed record CustomFieldDto(
    Guid Id,
    CustomFieldEntity EntityType,
    string Key,
    string Label,
    CustomFieldType Type,
    bool Required,
    IReadOnlyList<string> Options,
    int SortOrder,
    bool ShowInList)
{
    public static CustomFieldDto From(CustomFieldDefinition d) =>
        new(d.Id, d.EntityType, d.Key, d.Label, d.Type, d.Required, d.Options, d.SortOrder, d.ShowInList);
}
