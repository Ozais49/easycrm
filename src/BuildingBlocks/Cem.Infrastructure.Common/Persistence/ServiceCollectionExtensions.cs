using Cem.Infrastructure.Common.Auditing;
using Cem.Infrastructure.Common.Outbox;
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
    /// Requires a <see cref="TimeProvider"/> and the request context (<c>AddRequestContext</c>) in DI.
    /// </summary>
    /// <param name="schema">Same value as the context's <c>Schema</c>; also holds the migrations history table.</param>
    public static IServiceCollection AddCemDbContext<TContext>(
        this IServiceCollection services,
        string connectionString,
        string schema)
        where TContext : CemDbContextBase
    {
        // Scoped: the interceptors read the scoped tenant/user context.
        services.TryAddScoped<AuditInterceptor>();
        services.TryAddScoped<AuditableEntityInterceptor>();
        services.TryAddScoped<TenantStampInterceptor>();
        services.TryAddScoped<TenantSessionConnectionInterceptor>();
        services.TryAddSingleton<OutboxSignal>();
        services.TryAddScoped<OutboxSignalInterceptor>();

        services.AddDbContext<TContext>((serviceProvider, options) => options
            .UseCemNpgsql(connectionString, schema)
            // Interceptors run in the order given. Audit must come before the tenant stamp: the audit entries it adds
            // are tenant entities and get their TenantId from the stamp that follows.
            .AddInterceptors(
                serviceProvider.GetRequiredService<AuditInterceptor>(),
                serviceProvider.GetRequiredService<AuditableEntityInterceptor>(),
                serviceProvider.GetRequiredService<TenantStampInterceptor>(),
                serviceProvider.GetRequiredService<TenantSessionConnectionInterceptor>(),
                serviceProvider.GetRequiredService<OutboxSignalInterceptor>()));

        return services;
    }

    /// <summary>Npgsql, snake_case names and the per-module migrations history table. Shared with design-time factories.</summary>
    public static DbContextOptionsBuilder UseCemNpgsql(
        this DbContextOptionsBuilder options,
        string connectionString,
        string schema) => options
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", schema))
            .UseSnakeCaseNamingConvention();
}
