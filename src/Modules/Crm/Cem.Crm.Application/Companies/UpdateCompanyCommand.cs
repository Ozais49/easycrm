using System.Text.Json;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Companies;

/// <param name="OwnerUserId">Null keeps the current owner.</param>
/// <param name="Tags">Replaces the tags of the company; null keeps them.</param>
/// <param name="CustomFields">Replaces the values of the active custom fields; null keeps all values.</param>
public sealed record UpdateCompanyCommand(
    Guid Id,
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
    IReadOnlyDictionary<string, JsonElement>? CustomFields) : ICommand<Result<CompanyDto>>, ICompanyInput;
