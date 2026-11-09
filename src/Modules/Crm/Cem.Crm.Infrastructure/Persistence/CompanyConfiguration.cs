using Cem.Crm.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Crm.Infrastructure.Persistence;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");

        builder.Property(c => c.Name).HasMaxLength(200);
        builder.Property(c => c.Industry).HasMaxLength(100);
        builder.Property(c => c.Website).HasMaxLength(300);
        builder.Property(c => c.Phone).HasMaxLength(30);
        builder.Property(c => c.Email).HasMaxLength(254);
        builder.Property(c => c.PanNumber).HasMaxLength(9);
        builder.ComplexProperty(c => c.Address, AddressMapping.Configure);
        builder.Property(c => c.CustomFields).HasColumnType("jsonb").HasDefaultValueSql("'{}'::jsonb");

        builder.HasIndex(c => new { c.TenantId, c.Name }).HasDatabaseName("ix_companies_tenant_id_name");
        builder.HasIndex(c => new { c.TenantId, c.OwnerUserId }).HasDatabaseName("ix_companies_tenant_id_owner_user_id");
        builder.HasIndex(c => new { c.TenantId, c.TeamId }).HasDatabaseName("ix_companies_tenant_id_team_id");
    }
}
