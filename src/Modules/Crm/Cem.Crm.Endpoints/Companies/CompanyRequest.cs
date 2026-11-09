using System.Text.Json;
using Cem.Crm.Application;

namespace Cem.Crm.Endpoints.Companies;

/// <summary>Body for creating and updating a company. A missing owner means the current user (create) or no change (update).</summary>
public sealed record CompanyRequest(
    string Name,
    string? Industry,
    string? Website,
    string? Phone,
    string? Email,
    string? PanNumber,
    AddressDto? Address,
    Guid? OwnerUserId,
    Guid? TeamId,
    IReadOnlyList<string>? Tags,
    IReadOnlyDictionary<string, JsonElement>? CustomFields);
