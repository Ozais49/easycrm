using Cem.Settings.Domain;

namespace Cem.Settings.Endpoints.CustomFields;

/// <summary>Body for creating a custom field. A missing key is generated from the label.</summary>
public sealed record CreateCustomFieldRequest(
    CustomFieldEntity EntityType,
    string? Key,
    string Label,
    CustomFieldType Type,
    bool Required,
    IReadOnlyList<string>? Options,
    bool ShowInList);
