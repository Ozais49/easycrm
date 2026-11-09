using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Companies;

/// <summary>Takes up an unassigned company: the caller becomes its owner.</summary>
public sealed record ClaimCompanyCommand(Guid Id) : ICommand<Result<CompanyDto>>;
