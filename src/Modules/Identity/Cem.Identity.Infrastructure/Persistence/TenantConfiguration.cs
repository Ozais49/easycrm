using Cem.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Identity.Infrastructure.Persistence;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.Property(t => t.ClerkOrgId).HasMaxLength(100);
        builder.Property(t => t.Name).HasMaxLength(200);
        builder.Property(t => t.Slug).HasMaxLength(200);

        builder.HasIndex(t => t.ClerkOrgId).IsUnique().HasDatabaseName("ux_tenants_clerk_org_id");
    }
}
