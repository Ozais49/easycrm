using Cem.Crm.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Crm.Infrastructure.Persistence;

public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");

        builder.Property(t => t.Name).HasMaxLength(50);
        builder.Property(t => t.Color).HasConversion<string>().HasMaxLength(20);

        // The unique index on (tenant_id, lower(name)) is an expression index: the migration creates it with SQL.
    }
}
