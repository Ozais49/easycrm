using Cem.Identity.Application.Teams;
using Cem.Infrastructure.Common.Authorization;
using Cem.Infrastructure.Common.Http;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cem.Identity.Endpoints.Teams;

public static class TeamEndpoints
{
    public static IEndpointRouteBuilder MapTeams(this IEndpointRouteBuilder api)
    {
        var teams = api.MapGroup("/teams");

        teams.MapGet("/", async (ISender sender, CancellationToken cancellationToken, int page = 1, int pageSize = 25) =>
                (await sender.Send(new ListTeamsQuery(page, pageSize), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Read)
            .WithName("ListTeams")
            .Produces<PagedResult<TeamDto>>();

        teams.MapPost("/", async (TeamRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new CreateTeamCommand(request.Name), cancellationToken);
                return result.IsSuccess
                    ? TypedResults.Created($"/api/teams/{result.Value.Id}", result.Value)
                    : result.Error.ToHttpResult();
            })
            .RequirePermission(Scope.Settings, PermissionAction.Edit)
            .WithName("CreateTeam")
            .Produces<TeamDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        teams.MapPut("/{id:guid}", async (Guid id, TeamRequest request, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new RenameTeamCommand(id, request.Name), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Edit)
            .WithName("RenameTeam")
            .Produces<TeamDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        teams.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new DeleteTeamCommand(id), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Delete)
            .WithName("DeleteTeam")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        teams.MapPut("/{id:guid}/members", async (
                Guid id, SetTeamMembersRequest request, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new SetTeamMembersCommand(id, request.MembershipIds), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Edit)
            .WithName("SetTeamMembers")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return api;
    }
}
