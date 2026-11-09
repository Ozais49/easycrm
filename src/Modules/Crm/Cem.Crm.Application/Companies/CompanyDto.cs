using System.Text.Json;
using Cem.Crm.Application.Tags;

namespace Cem.Crm.Application.Companies;

/// <param name="CustomFields">Values of the active custom field definitions, keyed by field key.</param>
public sealed record CompanyDto(
    Guid Id,
    string Name,
    string? Industry,
    string? Website,
    string? Phone,
    string? Email,
    string? PanNumber,
    AddressDto Address,
    Guid? OwnerUserId,
    Guid? TeamId,
    IReadOnlyList<TagDto> Tags,
    IReadOnlyDictionary<string, JsonElement> CustomFields,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
