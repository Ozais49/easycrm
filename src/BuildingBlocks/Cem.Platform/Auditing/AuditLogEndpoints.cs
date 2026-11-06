using Cem.Infrastructure.Common.Authorization;
using Cem.Infrastructure.Common.Http;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cem.Platform.Auditing;

public static class AuditLogEndpoints
{
    /// <summary>Maps <c>GET /audit-log</c> onto the tenant-scoped <c>/api</c> group.</summary>
    public static IEndpointRouteBuilder MapAuditLog(this IEndpointRouteBuilder api)
    {
        api.MapGet("/audit-log", async (
                ISender sender,
                CancellationToken cancellationToken,
                string? entityType = null,
                Guid? entityId = null,
                Guid? userId = null,
                int page = 1,
                int pageSize = 25) =>
                (await sender.Send(new ListAuditLogQuery(entityType, entityId, userId, page, pageSize), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Read)
            .WithName("ListAuditLog")
            .Produces<PagedResult<AuditEntryDto>>();

        return api;
    }
}
