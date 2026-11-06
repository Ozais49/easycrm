using System.Linq.Expressions;
using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cem.Infrastructure.Common.Persistence;

/// <summary>
/// Base for every module DbContext: one schema per module, soft-delete filter on <see cref="ISoftDeletable"/>.
/// </summary>
public abstract class CemDbContextBase(DbContextOptions options) : DbContext(options)
{
    public const string SoftDeleteFilterName = "SoftDelete";

    /// <summary>The module's Postgres schema, e.g. <c>crm</c>.</summary>
    protected abstract string Schema { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Query filters are declared on root types only. Each concern uses a *named* filter so they stack.
            if (entityType.BaseType is not null)
            {
                continue;
            }

            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                ApplySoftDeleteFilter(modelBuilder, entityType);
            }

            // Extension point: the tenant filter ("Tenant") for ITenantEntity is added here in the tenancy feature.
        }
    }

    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder, IMutableEntityType entityType)
    {
        var entity = Expression.Parameter(entityType.ClrType, "e");
        var notDeleted = Expression.Equal(
            Expression.Property(entity, nameof(ISoftDeletable.DeletedAt)),
            Expression.Constant(null, typeof(DateTimeOffset?)));

        modelBuilder.Entity(entityType.ClrType)
            .HasQueryFilter(SoftDeleteFilterName, Expression.Lambda(notDeleted, entity));
    }
}

