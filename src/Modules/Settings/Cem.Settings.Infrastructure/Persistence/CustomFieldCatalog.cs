using Cem.Settings.Contracts;
using Microsoft.EntityFrameworkCore;
using DomainEntity = Cem.Settings.Domain.CustomFieldEntity;
using DomainType = Cem.Settings.Domain.CustomFieldType;

namespace Cem.Settings.Infrastructure.Persistence;

public sealed class CustomFieldCatalog(SettingsDbContext db) : ICustomFieldCatalog
{
    public async Task<IReadOnlyList<CustomFieldDefinitionInfo>> GetActiveAsync(
        CustomFieldEntity entityType, CancellationToken cancellationToken)
    {
        var entity = Enum.Parse<DomainEntity>(entityType.ToString());
        var rows = await db.CustomFieldDefinitions.AsNoTracking()
            .Where(d => d.EntityType == entity)
            .OrderBy(d => d.SortOrder).ThenBy(d => d.Key)
            .ToListAsync(cancellationToken);

        return rows
            .Select(d => new CustomFieldDefinitionInfo(
                d.Key, d.Label, Enum.Parse<CustomFieldType>(d.Type.ToString()), d.Required, d.Options))
            .ToList();
    }
}
