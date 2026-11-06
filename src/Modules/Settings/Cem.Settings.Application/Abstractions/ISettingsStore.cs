using Cem.Settings.Domain;

namespace Cem.Settings.Application.Abstractions;

/// <summary>Persistence port of the Settings module; settings are looked up within the current tenant.</summary>
public interface ISettingsStore
{
    /// <summary>The settings of the current tenant, tracked for update; null until they were created.</summary>
    Task<TenantSettings?> FindAsync(CancellationToken cancellationToken);

    /// <summary>The settings of the current tenant as a read-only copy; null until they were created.</summary>
    Task<TenantSettings?> FindReadOnlyAsync(CancellationToken cancellationToken);

    void Add(TenantSettings settings);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Saves a new settings row. Returns false when the row already exists (a concurrent insert hit the
    /// unique index); the rejected insert is dropped.
    /// </summary>
    Task<bool> TrySaveNewAsync(CancellationToken cancellationToken);
}
