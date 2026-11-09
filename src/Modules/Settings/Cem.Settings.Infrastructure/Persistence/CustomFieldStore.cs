using Cem.Settings.Application.Abstractions;
using Cem.Settings.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Cem.Settings.Infrastructure.Persistence;

public sealed class CustomFieldStore(SettingsDbContext db) : ICustomFieldStore
{
    public Task<CustomFieldDefinition?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        db.CustomFieldDefinitions.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<IReadOnlyList<CustomFieldDefinition>> ListActiveAsync(
        CustomFieldEntity entityType, CancellationToken cancellationToken) =>
        await db.CustomFieldDefinitions
            .Where(d => d.EntityType == entityType)
            .OrderBy(d => d.SortOrder).ThenBy(d => d.Key)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CustomFieldDefinition>> ListActiveReadOnlyAsync(
        CustomFieldEntity entityType, CancellationToken cancellationToken) =>
        await db.CustomFieldDefinitions.AsNoTracking()
            .Where(d => d.EntityType == entityType)
            .OrderBy(d => d.SortOrder).ThenBy(d => d.Key)
            .ToListAsync(cancellationToken);

    public void Add(CustomFieldDefinition definition) => db.CustomFieldDefinitions.Add(definition);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);

    public async Task<bool> TrySaveNewAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Drop only the rejected insert.
            foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList())
            {
                entry.State = EntityState.Detached;
            }

            return false;
        }
    }
}
