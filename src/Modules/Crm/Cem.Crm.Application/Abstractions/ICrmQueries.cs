using Cem.Crm.Application.Companies;
using Cem.Crm.Application.Contacts;
using Cem.SharedKernel;

namespace Cem.Crm.Application.Abstractions;

/// <summary>Read side of the Crm module: no-tracking projections, already limited to the records the caller may read.</summary>
public interface ICrmQueries
{
    Task<PagedResult<CompanyDto>> ListCompaniesAsync(
        RecordAccess access, string? search, int page, int pageSize, CancellationToken cancellationToken);

    Task<CompanyDto?> GetCompanyAsync(Guid id, RecordAccess access, CancellationToken cancellationToken);

    Task<PagedResult<ContactDto>> ListContactsAsync(
        RecordAccess access, string? search, Guid? companyId, int page, int pageSize, CancellationToken cancellationToken);

    Task<ContactDto?> GetContactAsync(Guid id, RecordAccess access, CancellationToken cancellationToken);
}
