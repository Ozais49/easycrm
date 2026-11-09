using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Companies;

/// <param name="OwnerUserId">Null keeps the current owner.</param>
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
    Guid? TeamId) : ICommand<Result<CompanyDto>>, ICompanyInput;
