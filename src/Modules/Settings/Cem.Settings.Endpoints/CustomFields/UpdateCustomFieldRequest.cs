namespace Cem.Settings.Endpoints.CustomFields;

/// <summary>Body for updating a custom field; key and type cannot change.</summary>
public sealed record UpdateCustomFieldRequest(string Label, bool Required, IReadOnlyList<string>? Options, bool ShowInList);
