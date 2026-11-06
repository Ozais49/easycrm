using Cem.Identity.Application.Abstractions;
using Cem.Identity.Domain;
using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Cem.Identity.Infrastructure.Persistence;

public sealed class IdentityStore(IdentityDbContext db) : IIdentityStore
{
    public Task<Tenant?> FindTenantAsync(string clerkOrgId, CancellationToken cancellationToken) =>
        db.Tenants.FirstOrDefaultAsync(t => t.ClerkOrgId == clerkOrgId, cancellationToken);

    public Task<User?> FindUserAsync(string clerkUserId, CancellationToken cancellationToken) =>
        db.Users.FirstOrDefaultAsync(u => u.ClerkUserId == clerkUserId, cancellationToken);

    public Task<TenantMembership?> FindMembershipAsync(Guid userId, CancellationToken cancellationToken) =>
        db.TenantMemberships.FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);

    public void Add(Entity entity) => db.Add(entity);

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
            // Drop only the rejected inserts so entities already loaded in this scope stay tracked.
            foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList())
            {
                entry.State = EntityState.Detached;
            }

            return false;
        }
    }
}
