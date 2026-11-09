using Cem.Crm.Domain;
using Cem.Infrastructure.Common.Persistence;
using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Cem.Crm.Infrastructure.Persistence;

public sealed class CrmDbContext(
    DbContextOptions<CrmDbContext> options,
    ITenantContext tenantContext,
    TimeProvider timeProvider) : CemDbContextBase(options, tenantContext, timeProvider)
{
    public const string SchemaName = "crm";

    protected override string Schema => SchemaName;

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<CompanyTag> CompanyTags => Set<CompanyTag>();
    public DbSet<ContactTag> ContactTags => Set<ContactTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
