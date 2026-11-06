using Cem.Infrastructure.Common.Auditing;
using Cem.Infrastructure.Common.Outbox;
using Cem.Infrastructure.Common.Persistence;
using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Cem.Platform.Persistence;

/// <summary>Owns the migrations of the shared platform tables and reads them (audit log, outbox dispatcher).</summary>
public sealed class PlatformDbContext(
    DbContextOptions<PlatformDbContext> options,
    ITenantContext tenantContext,
    TimeProvider timeProvider) : CemDbContextBase(options, tenantContext, timeProvider)
{
    protected override string Schema => PlatformSchema;

    protected override bool OwnsPlatformTables => true;

    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
}
