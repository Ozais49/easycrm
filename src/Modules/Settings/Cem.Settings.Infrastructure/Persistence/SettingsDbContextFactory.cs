using Cem.Infrastructure.Common.Persistence;
using Cem.Infrastructure.Common.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cem.Settings.Infrastructure.Persistence;

/// <summary>Used by <c>dotnet ef</c> only; never connects to the database when adding a migration.</summary>
public sealed class SettingsDbContextFactory : IDesignTimeDbContextFactory<SettingsDbContext>
{
    public SettingsDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SettingsDbContext>();
        builder.UseCemNpgsql("Host=localhost;Database=cem", SettingsDbContext.SchemaName);

        return new SettingsDbContext(builder.Options, new RequestContext(), TimeProvider.System);
    }
}
