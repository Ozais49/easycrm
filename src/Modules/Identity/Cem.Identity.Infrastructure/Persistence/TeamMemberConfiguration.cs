using Cem.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Identity.Infrastructure.Persistence;

public sealed class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.HasOne<Tenant>().WithMany().HasForeignKey(m => m.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Team>().WithMany().HasForeignKey(m => m.TeamId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<TenantMembership>().WithMany().HasForeignKey(m => m.MembershipId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.TenantId, m.TeamId, m.MembershipId }).IsUnique()
            .HasDatabaseName("ux_team_members_tenant_id_team_id_membership_id");
        builder.HasIndex(m => new { m.TenantId, m.MembershipId }).HasDatabaseName("ix_team_members_tenant_id_membership_id");
    }
}
