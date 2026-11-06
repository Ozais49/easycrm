using Cem.Infrastructure.Common.Persistence;
using Cem.Infrastructure.Common.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cem.Identity.Infrastructure.Persistence;

/// <summary>Used by <c>dotnet ef</c> only; never connects to the database when adding a migration.</summary>
public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<IdentityDbContext>();
        builder.UseCemNpgsql("Host=localhost;Database=cem", IdentityDbContext.SchemaName);

        return new IdentityDbContext(builder.Options, new RequestContext());
    }
}
