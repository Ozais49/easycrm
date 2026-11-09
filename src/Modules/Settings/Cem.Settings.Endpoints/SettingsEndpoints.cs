using Cem.Settings.Endpoints.CustomFields;
using Cem.Settings.Endpoints.Tenant;
using Microsoft.AspNetCore.Routing;

namespace Cem.Settings.Endpoints;

public static class SettingsEndpoints
{
    /// <summary>The tenant-scoped endpoints; map them onto the <c>/api</c> group that requires a tenant.</summary>
    public static IEndpointRouteBuilder MapSettingsApi(this IEndpointRouteBuilder api) =>
        api.MapTenantSettings().MapCustomFields();
}
