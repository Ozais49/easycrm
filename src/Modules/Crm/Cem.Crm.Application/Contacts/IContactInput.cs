namespace Cem.Crm.Application.Contacts;

/// <summary>The editable fields shared by creating and updating a contact; PII ones are null when not sent.</summary>
public interface IContactInput
{
    string FirstName { get; }
    string? LastName { get; }
    string? JobTitle { get; }
    Guid? CompanyId { get; }
    string? Email { get; }
    string? Phone { get; }
    string? Mobile { get; }
    AddressDto? Address { get; }
}
