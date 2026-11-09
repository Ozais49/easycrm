namespace Cem.Crm.Application.Companies;

/// <summary>The editable fields shared by creating and updating a company.</summary>
public interface ICompanyInput
{
    string Name { get; }
    string? Industry { get; }
    string? Website { get; }
    string? Phone { get; }
    string? Email { get; }
    string? PanNumber { get; }
    AddressDto? Address { get; }
}
