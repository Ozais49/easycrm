using Cem.Infrastructure.Common.Persistence;
using Cem.Infrastructure.Common.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cem.Crm.Infrastructure.Persistence;

/// <summary>Used by <c>dotnet ef</c> only; never connects to the database when adding a migration.</summary>
public sealed class CrmDbContextFactory : IDesignTimeDbContextFactory<CrmDbContext>
{
    public CrmDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<CrmDbContext>();
        builder.UseCemNpgsql("Host=localhost;Database=cem", CrmDbContext.SchemaName);

        return new CrmDbContext(builder.Options, new RequestContext(), TimeProvider.System);
    }
}
