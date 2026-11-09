namespace Cem.Settings.Contracts;

/// <param name="Options">The allowed values of a <see cref="CustomFieldType.Select"/> field; empty for other types.</param>
public sealed record CustomFieldDefinitionInfo(
    string Key,
    string Label,
    CustomFieldType Type,
    bool Required,
    IReadOnlyList<string> Options);
