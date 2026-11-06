using Cem.Settings.Application.Abstractions;
using Cem.Settings.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Cem.Settings.Infrastructure.Persistence;

public sealed class SettingsStore(SettingsDbContext db) : ISettingsStore
{
    public Task<TenantSettings?> FindAsync(CancellationToken cancellationToken) =>
        db.TenantSettings.FirstOrDefaultAsync(cancellationToken);

    public Task<TenantSettings?> FindReadOnlyAsync(CancellationToken cancellationToken) =>
        db.TenantSettings.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

    public void Add(TenantSettings settings) => db.TenantSettings.Add(settings);

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
