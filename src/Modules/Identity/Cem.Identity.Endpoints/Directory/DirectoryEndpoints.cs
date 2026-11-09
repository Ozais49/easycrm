using Cem.Identity.Application.Directory;
using Cem.Infrastructure.Common.Http;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cem.Identity.Endpoints.Directory;

public static class DirectoryEndpoints
{
    /// <summary>Pickers (owner, team): open to every member of the tenant, so no permission is required.</summary>
    public static IEndpointRouteBuilder MapDirectory(this IEndpointRouteBuilder api)
    {
        var directory = api.MapGroup("/directory");

        directory.MapGet("/members", async (ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new ListDirectoryMembersQuery(), cancellationToken)).ToHttpResult())
            .WithName("ListDirectoryMembers")
            .Produces<IReadOnlyList<DirectoryMemberDto>>();

        directory.MapGet("/teams", async (ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new ListDirectoryTeamsQuery(), cancellationToken)).ToHttpResult())
            .WithName("ListDirectoryTeams")
            .Produces<IReadOnlyList<DirectoryTeamDto>>();

        return api;
    }
}
