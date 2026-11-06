namespace Cem.SharedKernel;

/// <summary>
/// Audit columns. Stamped by the infrastructure layer on save; never set from domain or application code.
/// </summary>
public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
}
