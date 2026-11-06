using Cem.Infrastructure.Common.Persistence;
using Cem.Platform.Auditing;
using Cem.Platform.Outbox;
using Cem.Platform.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cem.Platform;

public static class PlatformModule
{
    /// <summary>Platform tables and the audit log query. The outbox dispatcher runs unless <c>Outbox:Enabled</c> is false.</summary>
    public static IServiceCollection AddPlatform(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Cem")
            ?? throw new InvalidOperationException("Connection string 'Cem' is missing.");

        services.AddCemDbContext<PlatformDbContext>(connectionString, CemDbContextBase.PlatformSchema);
        services.AddValidatorsFromAssemblyContaining<ListAuditLogValidator>(ServiceLifetime.Singleton);

        if (configuration.GetValue("Outbox:Enabled", true))
        {
            services.AddHostedService<OutboxDispatcher>();
        }

        return services;
    }

    /// <summary>The tenant-scoped endpoints; map them onto the <c>/api</c> group that requires a tenant.</summary>
    public static IEndpointRouteBuilder MapPlatformApi(this IEndpointRouteBuilder api) => api.MapAuditLog();
}
