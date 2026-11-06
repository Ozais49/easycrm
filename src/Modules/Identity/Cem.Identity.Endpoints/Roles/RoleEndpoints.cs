using Cem.Identity.Application.Roles;
using Cem.Infrastructure.Common.Authorization;
using Cem.Infrastructure.Common.Http;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cem.Identity.Endpoints.Roles;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoles(this IEndpointRouteBuilder api)
    {
        var roles = api.MapGroup("/roles");

        roles.MapGet("/", async (ISender sender, CancellationToken cancellationToken, int page = 1, int pageSize = 25) =>
                (await sender.Send(new ListRolesQuery(page, pageSize), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Read)
            .WithName("ListRoles")
            .Produces<PagedResult<RoleDto>>();

        roles.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new GetRoleQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Read)
            .WithName("GetRole")
            .Produces<RoleDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        roles.MapPost("/", async (RoleRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new CreateRoleCommand(request.Name, request.Permissions), cancellationToken);
                return result.IsSuccess
                    ? TypedResults.Created($"/api/roles/{result.Value.Id}", result.Value)
                    : result.Error.ToHttpResult();
            })
            .RequirePermission(Scope.Settings, PermissionAction.Edit)
            .WithName("CreateRole")
            .Produces<RoleDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        roles.MapPut("/{id:guid}", async (Guid id, RoleRequest request, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new UpdateRoleCommand(id, request.Name, request.Permissions), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Edit)
            .WithName("UpdateRole")
            .Produces<RoleDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        roles.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new DeleteRoleCommand(id), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Delete)
            .WithName("DeleteRole")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return api;
    }
}
