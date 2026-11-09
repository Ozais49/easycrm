using System.Text.Json;

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

    /// <summary>Tag names; null leaves the tags unchanged on update.</summary>
    IReadOnlyList<string>? Tags { get; }

    /// <summary>Custom field values by key; null leaves them unchanged on update.</summary>
    IReadOnlyDictionary<string, JsonElement>? CustomFields { get; }
}
