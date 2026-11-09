using Cem.Settings.Domain;

namespace Cem.Settings.Application.Abstractions;

/// <summary>Persistence port for custom field definitions; everything is looked up within the current tenant.</summary>
public interface ICustomFieldStore
{
    /// <summary>A definition of the current tenant, tracked for update; null when missing or deleted.</summary>
    Task<CustomFieldDefinition?> FindAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Active definitions of the entity type in display order, tracked for update.</summary>
    Task<IReadOnlyList<CustomFieldDefinition>> ListActiveAsync(CustomFieldEntity entityType, CancellationToken cancellationToken);

    /// <summary>Active definitions of the entity type in display order, as read-only copies.</summary>
    Task<IReadOnlyList<CustomFieldDefinition>> ListActiveReadOnlyAsync(
        CustomFieldEntity entityType, CancellationToken cancellationToken);

    void Add(CustomFieldDefinition definition);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>Saves a new definition. Returns false when its key was taken concurrently (unique index); the insert is dropped.</summary>
    Task<bool> TrySaveNewAsync(CancellationToken cancellationToken);
}
