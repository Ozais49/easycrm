using Cem.Infrastructure.Common.Persistence;
using Cem.Settings.Domain;
using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Cem.Settings.Infrastructure.Persistence;

public sealed class SettingsDbContext(
    DbContextOptions<SettingsDbContext> options,
    ITenantContext tenantContext,
    TimeProvider timeProvider) : CemDbContextBase(options, tenantContext, timeProvider)
{
    public const string SchemaName = "settings";

    protected override string Schema => SchemaName;

    public DbSet<TenantSettings> TenantSettings => Set<TenantSettings>();

    public DbSet<CustomFieldDefinition> CustomFieldDefinitions => Set<CustomFieldDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SettingsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
