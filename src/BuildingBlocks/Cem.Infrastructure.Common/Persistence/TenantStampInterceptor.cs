using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cem.Infrastructure.Common.Persistence;

/// <summary>
/// Stamps <see cref="ITenantEntity.TenantId"/> from the current tenant when an entity is added.
/// Changing it later is blocked by the model (<see cref="PropertySaveBehavior.Throw"/>, see <see cref="CemDbContextBase"/>).
/// </summary>
public sealed class TenantStampInterceptor(ITenantContext tenantContext) : SaveChangesInterceptor
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

        foreach (var entry in context.ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State != EntityState.Added)
            {
                continue;
            }

            var tenantId = tenantContext.TenantId
                ?? throw new InvalidOperationException(
                    $"Cannot add {entry.Metadata.ClrType.Name}: no tenant is set for this scope.");

            entry.Property(nameof(ITenantEntity.TenantId)).CurrentValue = tenantId;
        }
    }
}
