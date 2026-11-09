using Cem.Crm.Application;
using Cem.SharedKernel.Permissions;

namespace Cem.Crm.Endpoints.Contacts;

/// <summary>
/// Body for creating and updating a contact. Sending any contact PII property without ContactPii edit access
/// is rejected with 403 field.forbidden.
/// </summary>
public sealed record ContactRequest(
    string FirstName,
    string? LastName,
    string? JobTitle,
    Guid? CompanyId,
    [property: Sensitive(FieldGroup.ContactPii)] string? Email,
    [property: Sensitive(FieldGroup.ContactPii)] string? Phone,
    [property: Sensitive(FieldGroup.ContactPii)] string? Mobile,
    [property: Sensitive(FieldGroup.ContactPii)] AddressDto? Address,
    Guid? OwnerUserId,
    Guid? TeamId);
