using Cem.SharedKernel;

namespace Cem.Crm.Application;

internal static class CrmErrors
{
    public static readonly Error CompanyNotFound = Error.NotFound("companies.not_found", "Company not found.");

    public static readonly Error ContactNotFound = Error.NotFound("contacts.not_found", "Contact not found.");

    public static readonly Error TagNotFound = Error.NotFound("tags.not_found", "Tag not found.");

    public static readonly Error TagNameTaken = Invalid("Name", "Another tag already has this name.");

    /// <summary>Two requests created the same new tag at once; the caller can simply retry.</summary>
    public static readonly Error TagsChangedConcurrently =
        Error.Conflict("tags.conflict", "The tags were changed at the same time. Try again.");

    public static readonly Error PermissionDenied =
        Error.Forbidden("permission.denied", "You are not allowed to change this record.");

    /// <summary>Create at own/team level: the new record must stay within what the caller can reach.</summary>
    public static readonly Error OwnerOutsideCreateLevel = Invalid(
        "OwnerUserId", "You can only create records owned by yourself or assigned to one of your teams.");

    public static Error Invalid(string field, string message) =>
        Invalid(new Dictionary<string, string[]> { [field] = [message] });

    public static Error Invalid(IReadOnlyDictionary<string, string[]> fieldErrors) =>
        Error.Validation("validation.failed", "One or more validation errors occurred.", fieldErrors);
}
