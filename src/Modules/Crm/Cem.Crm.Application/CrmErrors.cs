using Cem.SharedKernel;

namespace Cem.Crm.Application;

internal static class CrmErrors
{
    public static readonly Error CompanyNotFound = Error.NotFound("companies.not_found", "Company not found.");

    public static readonly Error ContactNotFound = Error.NotFound("contacts.not_found", "Contact not found.");

    public static readonly Error PermissionDenied =
        Error.Forbidden("permission.denied", "You are not allowed to change this record.");

    public static Error Invalid(string field, string message) =>
        Error.Validation("validation.failed", "One or more validation errors occurred.",
            new Dictionary<string, string[]> { [field] = [message] });
}
