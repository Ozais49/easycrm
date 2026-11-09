using Cem.Crm.Application.Abstractions;
using Cem.Crm.Domain;
using Microsoft.EntityFrameworkCore;

namespace Cem.Crm.Infrastructure.Persistence;

public sealed class CrmStore(CrmDbContext db) : ICrmStore
{
    public Task<Company?> FindCompanyAsync(Guid id, CancellationToken cancellationToken) =>
        db.Companies.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<bool> CompanyExistsAsync(Guid id, CancellationToken cancellationToken) =>
        db.Companies.AsNoTracking().AnyAsync(c => c.Id == id, cancellationToken);

    public Task<Contact?> FindContactAsync(Guid id, CancellationToken cancellationToken) =>
        db.Contacts.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public void Add(Company company) => db.Companies.Add(company);

    public void Add(Contact contact) => db.Contacts.Add(contact);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
