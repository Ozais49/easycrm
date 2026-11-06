using Cem.Settings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Settings.Infrastructure.Persistence;

public sealed class TenantSettingsConfiguration : IEntityTypeConfiguration<TenantSettings>
{
    public void Configure(EntityTypeBuilder<TenantSettings> builder)
    {
        builder.Property(s => s.TimeZone).HasMaxLength(64);
        builder.Property(s => s.DateDisplay).HasConversion<string>().HasMaxLength(10);
        builder.Property(s => s.BrandColor).HasMaxLength(7);

        // Exactly one row per tenant.
        builder.HasIndex(s => s.TenantId).IsUnique().HasDatabaseName("ux_tenant_settings_tenant_id");
    }
}
