using Cem.Crm.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Crm.Infrastructure.Persistence;

public sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("contacts");

        builder.Property(c => c.FirstName).HasMaxLength(100);
        builder.Property(c => c.LastName).HasMaxLength(100);
        builder.Property(c => c.JobTitle).HasMaxLength(100);
        builder.Property(c => c.Email).HasMaxLength(254);
        builder.Property(c => c.Phone).HasMaxLength(30);
        builder.Property(c => c.Mobile).HasMaxLength(30);
        builder.ComplexProperty(c => c.Address, AddressMapping.Configure);

        // A deleted company stays referenced: its contacts keep the id.
        builder.HasOne<Company>().WithMany().HasForeignKey(c => c.CompanyId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.TenantId, c.CompanyId }).HasDatabaseName("ix_contacts_tenant_id_company_id");
        builder.HasIndex(c => new { c.TenantId, c.OwnerUserId }).HasDatabaseName("ix_contacts_tenant_id_owner_user_id");
        builder.HasIndex(c => new { c.TenantId, c.TeamId }).HasDatabaseName("ix_contacts_tenant_id_team_id");
    }
}
