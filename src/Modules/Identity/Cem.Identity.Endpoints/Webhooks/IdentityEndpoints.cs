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
}
