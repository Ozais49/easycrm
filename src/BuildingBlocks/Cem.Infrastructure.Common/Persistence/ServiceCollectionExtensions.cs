using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cem.Infrastructure.Common.Persistence;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a module DbContext on Npgsql. Pool size is set in the connection string (<c>Maximum Pool Size=10</c>).
    /// Tracking stays on (default) so command handlers can load, mutate and save; read paths must use
    /// <c>AsNoTracking()</c> with projections (see database-guidelines).
    /// Requires a <see cref="TimeProvider"/> in DI.
    /// </summary>
    /// <param name="schema">Same value as the context's <c>Schema</c>; also holds the migrations history table.</param>
    public static IServiceCollection AddCemDbContext<TContext>(
        this IServiceCollection services,
        string connectionString,
        string schema)
        where TContext : CemDbContextBase
    {
        services.TryAddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<TContext>((serviceProvider, options) => options
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", schema))
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>()));

        return services;
    }
}
