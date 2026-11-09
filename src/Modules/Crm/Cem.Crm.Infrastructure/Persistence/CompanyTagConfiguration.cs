using Cem.Crm.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Crm.Infrastructure.Persistence;

public sealed class CompanyTagConfiguration : IEntityTypeConfiguration<CompanyTag>
{
    public void Configure(EntityTypeBuilder<CompanyTag> builder)
    {
        builder.ToTable("company_tags");
        builder.HasKey(x => new { x.CompanyId, x.TagId });

        builder.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Tag>().WithMany().HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.TagId }).HasDatabaseName("ix_company_tags_tenant_id_tag_id");
    }
}
