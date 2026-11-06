namespace Cem.SharedKernel;

/// <summary>
/// Marks a tenant-owned entity.
/// </summary>
/// <remarks>
/// <see cref="TenantId"/> is read-only on the contract. Implementations declare it as
/// <c>{ get; private set; }</c>; the infrastructure layer stamps it once when the entity is Added
/// (via the EF property entry) and configures it to throw if modified afterwards, so it is immutable after insert.
/// </remarks>
public interface ITenantEntity
{
    Guid TenantId { get; }
}
