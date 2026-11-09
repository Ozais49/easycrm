using Cem.Settings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Settings.Infrastructure.Persistence;

public sealed class CustomFieldDefinitionConfiguration : IEntityTypeConfiguration<CustomFieldDefinition>
{
    public void Configure(EntityTypeBuilder<CustomFieldDefinition> builder)
    {
        builder.ToTable("custom_field_definitions");

        builder.Property(d => d.EntityType).HasConversion<string>().HasMaxLength(20);
        builder.Property(d => d.Key).HasMaxLength(40);
        builder.Property(d => d.Label).HasMaxLength(80);
        builder.Property(d => d.Type).HasConversion<string>().HasMaxLength(20);

        // A key is unique among the active definitions only; a deleted definition frees it.
        builder.HasIndex(d => new { d.TenantId, d.EntityType, d.Key })
            .IsUnique()
            .HasFilter("deleted_at IS NULL")
            .HasDatabaseName("ux_custom_field_definitions_tenant_id_entity_type_key");
    }
}
