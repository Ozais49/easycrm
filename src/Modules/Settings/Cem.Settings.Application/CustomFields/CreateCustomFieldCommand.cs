using Cem.Settings.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.CustomFields;

/// <param name="Key">Generated from the label when not given.</param>
public sealed record CreateCustomFieldCommand(
    CustomFieldEntity EntityType,
    string? Key,
    string Label,
    CustomFieldType Type,
    bool Required,
    IReadOnlyList<string>? Options,
    bool ShowInList) : ICommand<Result<CustomFieldDto>>;
