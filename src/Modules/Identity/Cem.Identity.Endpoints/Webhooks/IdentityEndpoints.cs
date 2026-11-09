using Cem.Identity.Endpoints.Directory;
using Cem.Identity.Endpoints.Me;
using Cem.Identity.Endpoints.Members;
using Cem.Identity.Endpoints.Roles;
using Cem.Identity.Endpoints.Teams;
using Cem.Identity.Endpoints.Webhooks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Cem.Identity.Endpoints;

public static class IdentityEndpoints
{
    public static IServiceCollection AddIdentityEndpoints(this IServiceCollection services) =>
        services.AddScoped<ClerkWebhookDispatcher>();

    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app) =>
        app.MapClerkWebhook();

    /// <summary>The tenant-scoped endpoints; map them onto the <c>/api</c> group that requires a tenant.</summary>
    public static IEndpointRouteBuilder MapIdentityApi(this IEndpointRouteBuilder api) =>
        api.MapMe().MapMembers().MapRoles().MapTeams().MapDirectory();
}
