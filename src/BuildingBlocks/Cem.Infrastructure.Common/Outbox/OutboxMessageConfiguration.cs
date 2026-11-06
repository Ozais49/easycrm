using Cem.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Infrastructure.Common.Outbox;

/// <param name="ownsMigrations">True only in the platform context; every other context maps the table without migrating it.</param>
public sealed class OutboxMessageConfiguration(bool ownsMigrations) : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", CemDbContextBase.PlatformSchema, table =>
        {
            if (!ownsMigrations)
            {
                table.ExcludeFromMigrations();
            }
        });

        builder.Property(m => m.EventType).HasMaxLength(200);
        builder.Property(m => m.Payload).HasColumnType("jsonb");

        // The dispatcher only ever asks for open messages that are due.
        builder.HasIndex(m => m.NextAttemptAt)
            .HasFilter("processed_at IS NULL AND dead_at IS NULL")
            .HasDatabaseName("ix_outbox_messages_next_attempt_at");
    }
}
