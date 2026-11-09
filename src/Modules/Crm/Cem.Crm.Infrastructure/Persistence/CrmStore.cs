using Cem.Crm.Application.Abstractions;
using Cem.Crm.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

    public async Task<IReadOnlyList<Tag>> ResolveTagsAsync(IReadOnlyList<string> names, CancellationToken cancellationToken)
    {
        if (names.Count == 0)
        {
            return [];
        }

        var lowered = names.Select(n => n.ToLowerInvariant()).ToList();
        var existing = (await db.Tags.Where(t => lowered.Contains(t.Name.ToLower())).ToListAsync(cancellationToken))
            .ToDictionary(t => t.Name.ToLowerInvariant());

        var tags = new List<Tag>(names.Count);
        foreach (var name in names)
        {
            if (!existing.TryGetValue(name.ToLowerInvariant(), out var tag))
            {
                tag = Tag.Create(name);
                db.Tags.Add(tag);
                existing[name.ToLowerInvariant()] = tag;
            }

            tags.Add(tag);
        }

        return tags;
    }

    public async Task SetTagsAsync(Company company, IReadOnlyList<Tag> tags, CancellationToken cancellationToken)
    {
        var current = db.Entry(company).State == EntityState.Added
            ? []
            : await db.CompanyTags.Where(x => x.CompanyId == company.Id).ToListAsync(cancellationToken);

        var wanted = tags.Select(t => t.Id).ToHashSet();
        db.CompanyTags.RemoveRange(current.Where(x => !wanted.Contains(x.TagId)));

        var present = current.Select(x => x.TagId).ToHashSet();
        db.CompanyTags.AddRange(wanted.Where(id => !present.Contains(id)).Select(id => CompanyTag.Create(company.Id, id)));
    }

    public async Task SetTagsAsync(Contact contact, IReadOnlyList<Tag> tags, CancellationToken cancellationToken)
    {
        var current = db.Entry(contact).State == EntityState.Added
            ? []
            : await db.ContactTags.Where(x => x.ContactId == contact.Id).ToListAsync(cancellationToken);

        var wanted = tags.Select(t => t.Id).ToHashSet();
        db.ContactTags.RemoveRange(current.Where(x => !wanted.Contains(x.TagId)));

        var present = current.Select(x => x.TagId).ToHashSet();
        db.ContactTags.AddRange(wanted.Where(id => !present.Contains(id)).Select(id => ContactTag.Create(contact.Id, id)));
    }

    public Task<Tag?> FindTagAsync(Guid id, CancellationToken cancellationToken) =>
        db.Tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<bool> TagNameTakenAsync(string name, Guid exceptId, CancellationToken cancellationToken)
    {
        var lowered = name.ToLowerInvariant();
        return db.Tags.AsNoTracking().AnyAsync(t => t.Id != exceptId && t.Name.ToLower() == lowered, cancellationToken);
    }

    public void Remove(Tag tag) => db.Tags.Remove(tag);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);

    public async Task<bool> TrySaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Drop the rejected inserts so the scope does not keep trying to save them.
            foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList())
            {
                entry.State = EntityState.Detached;
            }

            return false;
        }
    }
}
