using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Companies;

public sealed record GetCompanyQuery(Guid Id) : IQuery<Result<CompanyDto>>;
