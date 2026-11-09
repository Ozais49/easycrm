using Cem.Crm.Application.Tags;
using Cem.Infrastructure.Common.Authorization;
using Cem.Infrastructure.Common.Http;
using Cem.SharedKernel.Permissions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cem.Crm.Endpoints.Tags;

public static class TagEndpoints
{
    public static IEndpointRouteBuilder MapTags(this IEndpointRouteBuilder api)
    {
        var tags = api.MapGroup("/tags");

        // Tags are created through the Company and Contact endpoints; managing them is a settings task.
        tags.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new ListTagsQuery(), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Crm, PermissionAction.Read)
            .WithName("ListTags")
            .Produces<IReadOnlyList<TagDto>>();

        tags.MapPut("/{id:guid}", async (Guid id, TagRequest request, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new UpdateTagCommand(id, request.Name, request.Color), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Edit)
            .WithName("UpdateTag")
            .Produces<TagDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        tags.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new DeleteTagCommand(id), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Delete)
            .WithName("DeleteTag")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return api;
    }
}
