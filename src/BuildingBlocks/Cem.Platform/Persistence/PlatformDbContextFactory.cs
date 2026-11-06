using Cem.Infrastructure.Common.Persistence;
using Cem.Infrastructure.Common.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cem.Platform.Persistence;

/// <summary>Used by <c>dotnet ef</c> only; never connects to the database when adding a migration.</summary>
public sealed class PlatformDbContextFactory : IDesignTimeDbContextFactory<PlatformDbContext>
{
    public PlatformDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PlatformDbContext>();
        builder.UseCemNpgsql("Host=localhost;Database=cem", CemDbContextBase.PlatformSchema);

        return new PlatformDbContext(builder.Options, new RequestContext(), TimeProvider.System);
    }
}
