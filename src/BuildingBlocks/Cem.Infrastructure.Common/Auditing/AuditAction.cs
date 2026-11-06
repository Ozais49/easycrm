namespace Cem.Infrastructure.Common.Auditing;

public enum AuditAction
{
    Created,
    Updated,

    /// <summary>Hard delete or soft delete (<c>deleted_at</c> set).</summary>
    Deleted
}
