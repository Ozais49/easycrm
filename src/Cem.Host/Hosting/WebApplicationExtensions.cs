using Cem.Identity.Endpoints;
using Cem.Identity.Endpoints.Tenancy;
using Cem.Platform;

namespace Cem.Host.Hosting;

public static class WebApplicationExtensions
{
    public static WebApplication UseCemHost(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseTenantResolution();

        app.MapHealthChecks("/health").AllowAnonymous();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi().AllowAnonymous();
        }

        app.MapIdentityEndpoints();

        // Tenant-scoped API: every endpoint in this group has the tenant and user resolved from the Clerk token.
        // Modules map their endpoints onto it, e.g. api.MapCrmEndpoints();
        var api = app.MapGroup("/api").RequireTenant();
        api.MapIdentityApi();
        api.MapPlatformApi();

        return app;
    }
}
