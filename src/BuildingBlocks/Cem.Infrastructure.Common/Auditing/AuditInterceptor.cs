using System.Text.Json;
using System.Text.Json.Serialization;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cem.Infrastructure.Common.Auditing;

/// <summary>
/// Adds an <see cref="AuditEntry"/> for every added, modified or deleted <see cref="ITenantEntity"/>, in the same
/// SaveChanges, so the audit trail commits or rolls back with the change. Ids are client-generated (UUID v7), so
/// they are known before the save.
/// </summary>
/// <remarks>
/// Must run before <see cref="Persistence.TenantStampInterceptor"/>: the entries it adds get their tenant from that
/// stamp. The entity type is <c>schema.table</c> (e.g. <c>identity.roles</c>): unlike the CLR class name it only
/// changes through a deliberate migration, and it needs no pluralization rules.
/// </remarks>
public sealed class AuditInterceptor(TimeProvider timeProvider, ICurrentUser currentUser) : SaveChangesInterceptor
{
    // Bookkeeping, not business changes. TenantId is stamped on insert and immutable afterwards.
    private static readonly HashSet<string> IgnoredProperties =
    [
        nameof(AuditableEntity.CreatedAt),
        nameof(AuditableEntity.CreatedBy),
        nameof(AuditableEntity.UpdatedAt),
        nameof(AuditableEntity.UpdatedBy),
        nameof(ITenantEntity.TenantId)
    ];

    private static readonly Dictionary<string, bool> Redacted = new() { ["redacted"] = true };

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Audit(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Audit(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void Audit(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        // Materialized first: adding audit entries changes the tracker being enumerated.
        var changed = context.ChangeTracker.Entries<ITenantEntity>()
            .Where(e => e.Entity is not AuditEntry && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        var now = timeProvider.GetUtcNow();

        foreach (var entry in changed)
        {
            var action = ActionOf(entry);
            var changes = ChangesOf(entry, action);

            // An update that touched nothing but bookkeeping columns is not worth a row.
            if (action == AuditAction.Updated && changes.Count == 0)
            {
                continue;
            }

            context.Add(AuditEntry.Create(
                now,
                currentUser.UserId,
                action,
                EntityTypeName(entry.Metadata),
                (Guid)entry.Property(nameof(Entity.Id)).CurrentValue!,
                JsonSerializer.SerializeToElement(changes, Json)));
        }
    }

    private static AuditAction ActionOf(EntityEntry entry) => entry.State switch
    {
        EntityState.Added => AuditAction.Created,
        EntityState.Deleted => AuditAction.Deleted,
        _ => IsSoftDeleted(entry) ? AuditAction.Deleted : AuditAction.Updated
    };

    private static bool IsSoftDeleted(EntityEntry entry) =>
        entry.Entity is ISoftDeletable
        && entry.Property(nameof(ISoftDeletable.DeletedAt)) is { IsModified: true, OriginalValue: null, CurrentValue: not null };

    private static Dictionary<string, object?> ChangesOf(EntityEntry entry, AuditAction action)
    {
        var changes = new Dictionary<string, object?>();
        if (action == AuditAction.Deleted)
        {
            return changes;
        }

        foreach (var property in entry.Properties)
        {
            var metadata = property.Metadata;
            if (metadata.IsPrimaryKey()
                || IgnoredProperties.Contains(metadata.Name)
                || (action == AuditAction.Updated && !property.IsModified))
            {
                continue;
            }

            changes[JsonNamingPolicy.CamelCase.ConvertName(metadata.Name)] = IsSensitive(metadata)
                ? Redacted
                : action == AuditAction.Created
                    ? new Dictionary<string, object?> { ["new"] = property.CurrentValue }
                    : new Dictionary<string, object?> { ["old"] = property.OriginalValue, ["new"] = property.CurrentValue };
        }

        return changes;
    }

    // The change is recorded, the value never is.
    private static bool IsSensitive(IProperty property) =>
        property.PropertyInfo?.IsDefined(typeof(SensitiveAttribute), inherit: true) == true;

    private static string EntityTypeName(IEntityType entityType) =>
        $"{entityType.GetSchema() ?? entityType.Model.GetDefaultSchema()}.{entityType.GetTableName()}";
}
