using Cem.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Identity.Infrastructure.Persistence;

public sealed class MembershipRoleConfiguration : IEntityTypeConfiguration<MembershipRole>
{
    public void Configure(EntityTypeBuilder<MembershipRole> builder)
    {
        builder.HasOne<Tenant>().WithMany().HasForeignKey(m => m.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<TenantMembership>().WithMany().HasForeignKey(m => m.MembershipId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Role>().WithMany().HasForeignKey(m => m.RoleId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.TenantId, m.MembershipId, m.RoleId }).IsUnique()
            .HasDatabaseName("ux_membership_roles_tenant_id_membership_id_role_id");
        builder.HasIndex(m => new { m.TenantId, m.RoleId }).HasDatabaseName("ix_membership_roles_tenant_id_role_id");
    }
}
