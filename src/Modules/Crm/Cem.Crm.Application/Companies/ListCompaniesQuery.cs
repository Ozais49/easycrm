using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Companies;

public sealed record ListCompaniesQuery(int Page, int PageSize, string? Search) : IQuery<Result<PagedResult<CompanyDto>>>;
