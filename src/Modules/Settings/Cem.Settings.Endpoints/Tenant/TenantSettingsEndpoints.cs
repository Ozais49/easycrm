using Cem.Infrastructure.Common.Authorization;
using Cem.Infrastructure.Common.Http;
using Cem.Settings.Application.Tenant;
using Cem.SharedKernel.Permissions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Cem.Settings.Endpoints.Tenant;

public static class TenantSettingsEndpoints
{
    public static IEndpointRouteBuilder MapTenantSettings(this IEndpointRouteBuilder api)
    {
        var settings = api.MapGroup("/settings");

        // Open to every member: the UI needs the time zone, date display and brand color everywhere.
        settings.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetTenantSettingsQuery(), cancellationToken)).ToHttpResult());

        settings.MapPut("/", async (TenantSettingsRequest request, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(
                    new UpdateTenantSettingsCommand(request.TimeZone, request.DateDisplay, request.BrandColor),
                    cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Edit);

        return api;
    }
}
