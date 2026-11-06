using Cem.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Infrastructure.Common.Auditing;

/// <param name="ownsMigrations">True only in the platform context; every other context maps the table without migrating it.</param>
public sealed class AuditEntryConfiguration(bool ownsMigrations) : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.ToTable("audit_entries", CemDbContextBase.PlatformSchema, table =>
        {
            if (!ownsMigrations)
            {
                table.ExcludeFromMigrations();
            }
        });

        builder.Property(e => e.Action).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.EntityType).HasMaxLength(100);
        builder.Property(e => e.Changes).HasColumnType("jsonb");

        builder.HasIndex(e => new { e.TenantId, e.EntityType, e.EntityId, e.OccurredAt })
            .IsDescending(false, false, false, true)
            .HasDatabaseName("ix_audit_entries_tenant_id_entity_type_entity_id_occurred_at");
        builder.HasIndex(e => new { e.TenantId, e.OccurredAt })
            .IsDescending(false, true)
            .HasDatabaseName("ix_audit_entries_tenant_id_occurred_at");
    }
}
