using Cem.SharedKernel;

namespace Cem.Settings.Application.CustomFields;

internal static class CustomFieldErrors
{
    public const int MaxPerEntity = 50;

    public static readonly Error NotFound = Error.NotFound("custom_fields.not_found", "Custom field not found.");

    public static Error Invalid(string field, string message) =>
        Error.Validation("validation.failed", "One or more validation errors occurred.",
            new Dictionary<string, string[]> { [field] = [message] });
}
