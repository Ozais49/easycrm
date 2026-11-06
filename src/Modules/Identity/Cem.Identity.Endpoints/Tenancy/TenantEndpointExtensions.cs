using Microsoft.AspNetCore.Builder;

namespace Cem.Identity.Endpoints.Tenancy;

public static class TenantEndpointExtensions
{
    /// <summary>
    /// Marks every endpoint in the group as tenant-scoped; <see cref="TenantResolutionMiddleware"/> then resolves
    /// tenant and user (and loads permissions) before the endpoint runs.
    /// </summary>
    public static TBuilder RequireTenant<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder =>
        builder.WithMetadata(new TenantRequiredMetadata());

    /// <summary>Runs tenant resolution for endpoints marked with <see cref="RequireTenant{TBuilder}"/>. Place after authorization.</summary>
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app) =>
        app.UseMiddleware<TenantResolutionMiddleware>();
}
