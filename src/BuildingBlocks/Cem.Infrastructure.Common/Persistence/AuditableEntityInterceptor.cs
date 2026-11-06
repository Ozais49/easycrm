using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cem.Infrastructure.Common.Persistence;

/// <summary>Stamps <see cref="AuditableEntity"/> timestamps on save. User ids are stamped in a later feature.</summary>
public sealed class AuditableEntityInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Stamp(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void Stamp(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            // Audit setters are private; write through the EF property entry instead.
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(AuditableEntity.CreatedAt)).CurrentValue = now;
                    break;
                case EntityState.Modified:
                    entry.Property(nameof(AuditableEntity.UpdatedAt)).CurrentValue = now;
                    break;
            }
        }
    }
}
