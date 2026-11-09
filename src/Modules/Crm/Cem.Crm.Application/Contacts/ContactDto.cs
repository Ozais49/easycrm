using Cem.SharedKernel.Permissions;

namespace Cem.Crm.Application.Contacts;

/// <summary>The contact PII properties are absent from the response for callers without ContactPii read access.</summary>
public sealed record ContactDto(
    Guid Id,
    string FirstName,
    string? LastName,
    string? JobTitle,
    Guid? CompanyId,
    string? CompanyName,
    [property: Sensitive(FieldGroup.ContactPii)] string? Email,
    [property: Sensitive(FieldGroup.ContactPii)] string? Phone,
    [property: Sensitive(FieldGroup.ContactPii)] string? Mobile,
    [property: Sensitive(FieldGroup.ContactPii)] AddressDto? Address,
    Guid OwnerUserId,
    Guid? TeamId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
