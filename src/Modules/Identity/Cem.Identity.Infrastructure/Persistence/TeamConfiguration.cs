using Cem.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Identity.Infrastructure.Persistence;

public sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(100);

        builder.HasOne<Tenant>().WithMany().HasForeignKey(t => t.TenantId).OnDelete(DeleteBehavior.Restrict);

        // A deleted team frees its name.
        builder.HasIndex(t => new { t.TenantId, t.Name }).IsUnique()
            .HasFilter("deleted_at is null")
            .HasDatabaseName("ux_teams_tenant_id_name");
    }
}
