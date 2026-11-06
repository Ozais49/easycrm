using Cem.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Identity.Infrastructure.Persistence;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.ClerkUserId).HasMaxLength(100);
        builder.Property(u => u.Email).HasMaxLength(320);
        builder.Property(u => u.FirstName).HasMaxLength(200);
        builder.Property(u => u.LastName).HasMaxLength(200);
        builder.Property(u => u.ImageUrl).HasMaxLength(2048);

        builder.HasIndex(u => u.ClerkUserId).IsUnique().HasDatabaseName("ux_users_clerk_user_id");
    }
}
