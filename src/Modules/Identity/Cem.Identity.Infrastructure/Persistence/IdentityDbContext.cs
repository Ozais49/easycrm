using Cem.Identity.Domain;
using Cem.Infrastructure.Common.Persistence;
using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Cem.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options,
    ITenantContext tenantContext,
    TimeProvider timeProvider) : CemDbContextBase(options, tenantContext, timeProvider)
{
    public const string SchemaName = "identity";

    protected override string Schema => SchemaName;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<MembershipRole> MembershipRoles => Set<MembershipRole>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
