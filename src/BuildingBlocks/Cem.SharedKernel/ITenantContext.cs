namespace Cem.SharedKernel;

/// <summary>The tenant the current request or job is acting for.</summary>
public interface ITenantContext
{
    Guid? TenantId { get; }

    bool HasTenant { get; }
}
