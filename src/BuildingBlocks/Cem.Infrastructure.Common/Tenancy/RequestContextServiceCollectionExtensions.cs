using Cem.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace Cem.Infrastructure.Common.Tenancy;

public static class RequestContextServiceCollectionExtensions
{
    /// <summary>Registers the scoped <see cref="RequestContext"/> under all its interfaces.</summary>
    public static IServiceCollection AddRequestContext(this IServiceCollection services)
    {
        services.AddScoped<RequestContext>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<RequestContext>());
        services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<RequestContext>());
        services.AddScoped<IRequestContextSetter>(sp => sp.GetRequiredService<RequestContext>());
        return services;
    }
}
