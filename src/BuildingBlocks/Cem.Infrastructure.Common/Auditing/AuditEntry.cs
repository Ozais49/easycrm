using System.Text.Json;
using Cem.SharedKernel;

namespace Cem.Infrastructure.Common.Auditing;

/// <summary>
/// One recorded change of a tenant entity. Written by <see cref="AuditInterceptor"/> in the same transaction as the
/// change itself. Lives in the shared <c>platform</c> schema; every module context maps it.
/// </summary>
public sealed class AuditEntry : Entity, ITenantEntity
{
    private AuditEntry()
    {
    }

    public Guid TenantId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }

    /// <summary>Null for system changes (webhooks, background work).</summary>
    public Guid? UserId { get; private set; }

    public AuditAction Action { get; private set; }

    /// <summary>Schema and table of the entity, e.g. <c>identity.roles</c>.</summary>
    public string EntityType { get; private set; } = "";

    public Guid EntityId { get; private set; }

    /// <summary>
    /// <c>{ "property": { "old": ..., "new": ... } }</c>: all properties (new only) for Created, the changed ones for
    /// Updated, empty for Deleted. <c>[Sensitive]</c> properties show <c>{ "redacted": true }</c> instead.
    /// </summary>
    public JsonElement Changes { get; private set; }

    public static AuditEntry Create(
        DateTimeOffset occurredAt,
        Guid? userId,
        AuditAction action,
        string entityType,
        Guid entityId,
        JsonElement changes) =>
        new()
        {
            OccurredAt = occurredAt,
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Changes = changes
        };
}
