namespace Cem.Settings.Contracts;

/// <summary>Lets other modules read the custom field definitions of the current tenant.</summary>
public interface ICustomFieldCatalog
{
    /// <summary>The active (not deleted) definitions of the entity type, in display order.</summary>
    Task<IReadOnlyList<CustomFieldDefinitionInfo>> GetActiveAsync(
        CustomFieldEntity entityType, CancellationToken cancellationToken);
}
