using System.Linq.Expressions;
using System.Reflection;
using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cem.Infrastructure.Common.Persistence;

/// <summary>
/// Base for every module DbContext: one schema per module, soft-delete filter on <see cref="ISoftDeletable"/>,
/// tenant filter on <see cref="ITenantEntity"/>.
/// </summary>
public abstract class CemDbContextBase(DbContextOptions options, ITenantContext tenantContext) : DbContext(options)
{
    public const string SoftDeleteFilterName = "SoftDelete";
    public const string TenantFilterName = "Tenant";

    private static readonly PropertyInfo TenantFilterIdProperty =
        typeof(CemDbContextBase).GetProperty(nameof(TenantFilterId), BindingFlags.Instance | BindingFlags.NonPublic)!;

    /// <summary>The module's Postgres schema, e.g. <c>crm</c>.</summary>
    protected abstract string Schema { get; }

    /// <summary>
    /// The tenant id the query filter compares against. With no tenant it is <see cref="Guid.Empty"/>,
    /// which no row has (ids are UUID v7), so an unscoped context sees nothing rather than everything.
    /// </summary>
    private Guid TenantFilterId => tenantContext.TenantId ?? Guid.Empty;

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

            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                ApplyTenantFilter(modelBuilder, entityType);
            }
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

    private void ApplyTenantFilter(ModelBuilder modelBuilder, IMutableEntityType entityType)
    {
        // e => e.TenantId == this.TenantFilterId; EF re-reads the context member for every query.
        var entity = Expression.Parameter(entityType.ClrType, "e");
        var inCurrentTenant = Expression.Equal(
            Expression.Property(entity, nameof(ITenantEntity.TenantId)),
            Expression.Property(Expression.Constant(this), TenantFilterIdProperty));

        modelBuilder.Entity(entityType.ClrType)
            .HasQueryFilter(TenantFilterName, Expression.Lambda(inCurrentTenant, entity));

        // TenantId is stamped on insert only; any later change throws on save.
        entityType.FindProperty(nameof(ITenantEntity.TenantId))!
            .SetAfterSaveBehavior(PropertySaveBehavior.Throw);
    }
}
