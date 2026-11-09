using System.Linq.Expressions;
using System.Text.Json;
using Cem.Crm.Application;
using Cem.Crm.Application.Abstractions;
using Cem.Crm.Application.Companies;
using Cem.Crm.Application.Contacts;
using Cem.Crm.Application.Tags;
using Cem.Crm.Domain;
using Cem.Infrastructure.Common.Persistence;
using Cem.Settings.Contracts;
using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Cem.Crm.Infrastructure.Persistence;

public sealed class CrmQueries(CrmDbContext db, ICustomFieldCatalog catalog) : ICrmQueries
{
    private const int MaxTags = 500;

    // Tags and custom field values are filled in after the query (one extra query per page), so the projection
    // carries placeholders and the raw custom_fields json next to the dto.
    private static readonly IReadOnlyList<TagDto> NoTags = [];
    private static readonly IReadOnlyDictionary<string, JsonElement> NoFields = new Dictionary<string, JsonElement>();

    private sealed record Row<T>(T Dto, string CustomFields);

    private static readonly Expression<Func<Company, Row<CompanyDto>>> ToCompanyRow = c => new Row<CompanyDto>(
        new CompanyDto(
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
            NoTags,
            NoFields,
            c.CreatedAt,
            c.UpdatedAt),
        c.CustomFields);

    public async Task<PagedResult<CompanyDto>> ListCompaniesAsync(
        RecordAccess access, string? search, Guid? tagId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var companies = Companies(access, search);
        if (tagId is { } tag)
        {
            companies = companies.Where(c => db.CompanyTags.Any(x => x.CompanyId == c.Id && x.TagId == tag));
        }

        var rows = await companies
            .OrderBy(c => c.Name).ThenBy(c => c.Id)
            .Select(ToCompanyRow)
            .ToPagedAsync(page, pageSize, cancellationToken);

        return new PagedResult<CompanyDto>(await FillCompaniesAsync(rows.Items, cancellationToken), rows.Page, rows.PageSize, rows.Total);
    }

    public async Task<CompanyDto?> GetCompanyAsync(Guid id, RecordAccess access, CancellationToken cancellationToken)
    {
        var row = await Companies(access, null)
            .Where(c => c.Id == id)
            .Select(ToCompanyRow)
            .FirstOrDefaultAsync(cancellationToken);

        return row is null ? null : (await FillCompaniesAsync([row], cancellationToken))[0];
    }

    public async Task<PagedResult<ContactDto>> ListContactsAsync(
        RecordAccess access, string? search, Guid? companyId, Guid? tagId, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var contacts = Contacts(access, search);
        if (companyId is { } company)
        {
            contacts = contacts.Where(c => c.CompanyId == company);
        }

        if (tagId is { } tag)
        {
            contacts = contacts.Where(c => db.ContactTags.Any(x => x.ContactId == c.Id && x.TagId == tag));
        }

        var rows = await contacts
            .OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ThenBy(c => c.Id)
            .Select(ToContactRow())
            .ToPagedAsync(page, pageSize, cancellationToken);

        return new PagedResult<ContactDto>(await FillContactsAsync(rows.Items, cancellationToken), rows.Page, rows.PageSize, rows.Total);
    }

    public async Task<ContactDto?> GetContactAsync(Guid id, RecordAccess access, CancellationToken cancellationToken)
    {
        var row = await Contacts(access, null)
            .Where(c => c.Id == id)
            .Select(ToContactRow())
            .FirstOrDefaultAsync(cancellationToken);

        return row is null ? null : (await FillContactsAsync([row], cancellationToken))[0];
    }

    public async Task<IReadOnlyList<TagDto>> ListTagsAsync(CancellationToken cancellationToken) =>
        await db.Tags.AsNoTracking()
            .OrderBy(t => t.Name).ThenBy(t => t.Id)
            .Take(MaxTags)
            .Select(t => new TagDto(t.Id, t.Name, t.Color))
            .ToListAsync(cancellationToken);

    private async Task<List<CompanyDto>> FillCompaniesAsync(IReadOnlyList<Row<CompanyDto>> rows, CancellationToken cancellationToken)
    {
        if (rows.Count == 0)
        {
            return [];
        }

        var ids = rows.Select(r => r.Dto.Id).ToList();
        var tags = (await (
                from link in db.CompanyTags.AsNoTracking()
                where ids.Contains(link.CompanyId)
                join tag in db.Tags.AsNoTracking() on link.TagId equals tag.Id
                orderby tag.Name
                select new { link.CompanyId, Tag = new TagDto(tag.Id, tag.Name, tag.Color) })
            .ToListAsync(cancellationToken))
            .ToLookup(x => x.CompanyId, x => x.Tag);
        var definitions = await catalog.GetActiveAsync(CustomFieldEntity.Company, cancellationToken);

        return rows
            .Select(r => r.Dto with
            {
                Tags = tags[r.Dto.Id].ToList(),
                CustomFields = CustomFieldValues.ForRead(r.CustomFields, definitions)
            })
            .ToList();
    }

    private async Task<List<ContactDto>> FillContactsAsync(IReadOnlyList<Row<ContactDto>> rows, CancellationToken cancellationToken)
    {
        if (rows.Count == 0)
        {
            return [];
        }

        var ids = rows.Select(r => r.Dto.Id).ToList();
        var tags = (await (
                from link in db.ContactTags.AsNoTracking()
                where ids.Contains(link.ContactId)
                join tag in db.Tags.AsNoTracking() on link.TagId equals tag.Id
                orderby tag.Name
                select new { link.ContactId, Tag = new TagDto(tag.Id, tag.Name, tag.Color) })
            .ToListAsync(cancellationToken))
            .ToLookup(x => x.ContactId, x => x.Tag);
        var definitions = await catalog.GetActiveAsync(CustomFieldEntity.Contact, cancellationToken);

        return rows
            .Select(r => r.Dto with
            {
                Tags = tags[r.Dto.Id].ToList(),
                CustomFields = CustomFieldValues.ForRead(r.CustomFields, definitions)
            })
            .ToList();
    }

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
    private Expression<Func<Contact, Row<ContactDto>>> ToContactRow() => c => new Row<ContactDto>(
        new ContactDto(
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
            NoTags,
            NoFields,
            c.CreatedAt,
            c.UpdatedAt),
        c.CustomFields);
}
