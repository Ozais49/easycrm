using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.CustomFields;

/// <summary>Key and type are immutable and therefore not part of the update.</summary>
public sealed record UpdateCustomFieldCommand(
    Guid Id, string Label, bool Required, IReadOnlyList<string>? Options, bool ShowInList)
    : ICommand<Result<CustomFieldDto>>;
