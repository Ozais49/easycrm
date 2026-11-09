using System.Text.Json;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Companies;

/// <param name="OwnerUserId">Null leaves the record unassigned (the web form defaults to the current user).</param>
public sealed record CreateCompanyCommand(
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
