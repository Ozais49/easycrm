using System.Linq.Expressions;
using Cem.Crm.Application;
using Cem.Crm.Application.Abstractions;
using Cem.Crm.Application.Companies;
using Cem.Crm.Application.Contacts;
using Cem.Crm.Domain;
using Cem.Infrastructure.Common.Persistence;
using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Cem.Crm.Infrastructure.Persistence;

public sealed class CrmQueries(CrmDbContext db) : ICrmQueries
{
    private static readonly Expression<Func<Company, CompanyDto>> ToCompanyDto = c => new CompanyDto(
        c.Id,
        c.Name,
        c.Industry,
        c.Website,
        c.Phone,
        c.Email,
        c.PanNumber,
        new AddressDto(c.Address.Street, c.Address.City, c.Address.District, c.Address.Province),
        c.OwnerUserId,
        c.TeamId,
        c.CreatedAt,
        c.UpdatedAt);

    public Task<PagedResult<CompanyDto>> ListCompaniesAsync(
        RecordAccess access, string? search, int page, int pageSize, CancellationToken cancellationToken) =>
        Companies(access, search)
            .OrderBy(c => c.Name).ThenBy(c => c.Id)
            .Select(ToCompanyDto)
            .ToPagedAsync(page, pageSize, cancellationToken);

    public Task<CompanyDto?> GetCompanyAsync(Guid id, RecordAccess access, CancellationToken cancellationToken) =>
        Companies(access, null)
            .Where(c => c.Id == id)
            .Select(ToCompanyDto)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<PagedResult<ContactDto>> ListContactsAsync(
        RecordAccess access, string? search, Guid? companyId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var contacts = Contacts(access, search);
        if (companyId is { } company)
        {
            contacts = contacts.Where(c => c.CompanyId == company);
        }

        return contacts
            .OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ThenBy(c => c.Id)
            .Select(ToContactDto())
            .ToPagedAsync(page, pageSize, cancellationToken);
    }

    public Task<ContactDto?> GetContactAsync(Guid id, RecordAccess access, CancellationToken cancellationToken) =>
        Contacts(access, null)
            .Where(c => c.Id == id)
            .Select(ToContactDto())
            .FirstOrDefaultAsync(cancellationToken);

    private IQueryable<Company> Companies(RecordAccess access, string? search)
    {
        var companies = db.Companies.AsNoTracking().Where(access.Filter<Company>());
        if (search is null)
        {
            return companies;
        }

        var pattern = LikePattern.Contains(search);
        return companies.Where(c => EF.Functions.ILike(c.Name, pattern));
    }

    private IQueryable<Contact> Contacts(RecordAccess access, string? search)
    {
        var contacts = db.Contacts.AsNoTracking().Where(access.Filter<Contact>());
        if (search is null)
        {
            return contacts;
        }

        var pattern = LikePattern.Contains(search);
        return contacts.Where(c => EF.Functions.ILike(c.FirstName + " " + (c.LastName ?? ""), pattern));
    }

    // The company name comes from a subquery: a deleted company drops out through its query filter, so it shows nothing.
    private Expression<Func<Contact, ContactDto>> ToContactDto() => c => new ContactDto(
        c.Id,
        c.FirstName,
        c.LastName,
        c.JobTitle,
        c.CompanyId,
        db.Companies.Where(co => co.Id == c.CompanyId).Select(co => co.Name).FirstOrDefault(),
        c.Email,
        c.Phone,
        c.Mobile,
        new AddressDto(c.Address.Street, c.Address.City, c.Address.District, c.Address.Province),
        c.OwnerUserId,
        c.TeamId,
        c.CreatedAt,
        c.UpdatedAt);
}
