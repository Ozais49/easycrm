using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cem.Identity.Endpoints.Tenancy;

public static class TenantEndpointExtensions
{
    /// <summary>Resolves tenant and user for every endpoint in the group; they are available through the request context.</summary>
    public static TBuilder RequireTenant<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder =>
        builder.AddEndpointFilter<TBuilder, TenantResolutionFilter>();
}
