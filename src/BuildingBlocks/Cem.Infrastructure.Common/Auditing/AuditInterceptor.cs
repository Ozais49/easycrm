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
                EntityIdOf(entry),
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

    /// <summary>
    /// The id of the audited row. A link row (composite key, no <c>Id</c>, e.g. <c>crm.company_tags</c>) is audited
    /// under its first key value, the owning record, so the changes show up in that record's history.
    /// </summary>
    private static Guid EntityIdOf(EntityEntry entry)
    {
        var key = entry.Metadata.FindProperty(nameof(Entity.Id)) ?? entry.Metadata.FindPrimaryKey()!.Properties[0];
        return (Guid)entry.Property(key.Name).CurrentValue!;
    }

    private static Dictionary<string, object?> ChangesOf(EntityEntry entry, AuditAction action)
    {
        var changes = new Dictionary<string, object?>();

        // A link row has nothing but its key, so the key is the change (also when the row is removed).
        var isLink = entry.Metadata.FindProperty(nameof(Entity.Id)) is null;
        if (action == AuditAction.Deleted && !isLink)
        {
            return changes;
        }

        foreach (var property in entry.Properties)
        {
            var metadata = property.Metadata;
            if ((metadata.IsPrimaryKey() && !isLink)
                || IgnoredProperties.Contains(metadata.Name)
                || (action == AuditAction.Updated && !property.IsModified))
            {
                continue;
            }

            changes[JsonNamingPolicy.CamelCase.ConvertName(metadata.Name)] = IsSensitive(metadata.PropertyInfo)
                ? Redacted
                : Change(action, property.OriginalValue, property.CurrentValue);
        }

        // Complex properties (e.g. Address) are not part of entry.Properties: their members are listed as "address.city".
        foreach (var complex in entry.ComplexProperties.Where(c => !c.Metadata.IsCollection))
        {
            var name = JsonNamingPolicy.CamelCase.ConvertName(complex.Metadata.Name);
            var members = complex.Properties
                .Where(m => action == AuditAction.Created || (m.IsModified && !Equals(m.OriginalValue, m.CurrentValue)))
                .ToList();
            if (members.Count == 0)
            {
                continue;
            }

            if (IsSensitive(complex.Metadata.PropertyInfo))
            {
                changes[name] = Redacted;
                continue;
            }

            foreach (var member in members)
            {
                changes[$"{name}.{JsonNamingPolicy.CamelCase.ConvertName(member.Metadata.Name)}"] =
                    Change(action, member.OriginalValue, member.CurrentValue);
            }
        }

        return changes;
    }

    private static Dictionary<string, object?> Change(AuditAction action, object? original, object? current) =>
        action switch
        {
            AuditAction.Created => new Dictionary<string, object?> { ["new"] = current },
            AuditAction.Deleted => new Dictionary<string, object?> { ["old"] = original },
            _ => new Dictionary<string, object?> { ["old"] = original, ["new"] = current }
        };

    // The change is recorded, the value never is.
    private static bool IsSensitive(System.Reflection.PropertyInfo? property) =>
        property?.IsDefined(typeof(SensitiveAttribute), inherit: true) == true;

    private static string EntityTypeName(IEntityType entityType) =>
        $"{entityType.GetSchema() ?? entityType.Model.GetDefaultSchema()}.{entityType.GetTableName()}";
}
