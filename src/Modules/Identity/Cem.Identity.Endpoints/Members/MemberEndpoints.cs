using Cem.Identity.Application.Members;
using Cem.Infrastructure.Common.Authorization;
using Cem.Infrastructure.Common.Http;
using Cem.SharedKernel.Permissions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Cem.Identity.Endpoints.Members;

public static class MemberEndpoints
{
    public static IEndpointRouteBuilder MapMembers(this IEndpointRouteBuilder api)
    {
        var members = api.MapGroup("/members");

        members.MapGet("/", async (ISender sender, CancellationToken cancellationToken, int page = 1, int pageSize = 25) =>
                (await sender.Send(new ListMembersQuery(page, pageSize), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Read);

        members.MapPut("/{membershipId:guid}/roles", async (
                Guid membershipId, SetMemberRolesRequest request, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new SetMemberRolesCommand(membershipId, request.RoleIds), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Edit);

        return api;
    }
}
