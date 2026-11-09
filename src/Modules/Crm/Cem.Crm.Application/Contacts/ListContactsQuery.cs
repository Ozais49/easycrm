using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Contacts;

public sealed record ListContactsQuery(int Page, int PageSize, string? Search, Guid? CompanyId, Guid? TagId)
    : IQuery<Result<PagedResult<ContactDto>>>;
