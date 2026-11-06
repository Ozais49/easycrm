using Cem.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Identity.Infrastructure.Persistence;

public sealed class TenantMembershipConfiguration : IEntityTypeConfiguration<TenantMembership>
{
    public void Configure(EntityTypeBuilder<TenantMembership> builder)
    {
        builder.Property(m => m.ClerkMembershipId).HasMaxLength(100);
        builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne<Tenant>().WithMany().HasForeignKey(m => m.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.TenantId, m.UserId }).IsUnique().HasDatabaseName("ux_tenant_memberships_tenant_id_user_id");
        builder.HasIndex(m => m.ClerkMembershipId).IsUnique().HasDatabaseName("ux_tenant_memberships_clerk_membership_id");
        builder.HasIndex(m => m.UserId).HasDatabaseName("ix_tenant_memberships_user_id");
    }
}
