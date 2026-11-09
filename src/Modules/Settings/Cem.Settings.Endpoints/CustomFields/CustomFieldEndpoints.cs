using Cem.Infrastructure.Common.Authorization;
using Cem.Infrastructure.Common.Http;
using Cem.Settings.Application.CustomFields;
using Cem.Settings.Domain;
using Cem.SharedKernel.Permissions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cem.Settings.Endpoints.CustomFields;

public static class CustomFieldEndpoints
{
    public static IEndpointRouteBuilder MapCustomFields(this IEndpointRouteBuilder api)
    {
        var fields = api.MapGroup("/custom-fields");

        // Open to every member: record forms and lists need the definitions.
        fields.MapGet("/", async (CustomFieldEntity entityType, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new ListCustomFieldsQuery(entityType), cancellationToken)).ToHttpResult())
            .WithName("ListCustomFields")
            .Produces<IReadOnlyList<CustomFieldDto>>()
            .ProducesValidationProblem();

        fields.MapPost("/", async (CreateCustomFieldRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new CreateCustomFieldCommand(
                        request.EntityType, request.Key, request.Label, request.Type, request.Required,
                        request.Options, request.ShowInList),
                    cancellationToken);
                return result.IsSuccess
                    ? TypedResults.Created($"/api/custom-fields/{result.Value.Id}", result.Value)
                    : result.Error.ToHttpResult();
            })
            .RequirePermission(Scope.Settings, PermissionAction.Edit)
            .WithName("CreateCustomField")
            .Produces<CustomFieldDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        fields.MapPut("/{id:guid}", async (
                Guid id, UpdateCustomFieldRequest request, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(
                    new UpdateCustomFieldCommand(id, request.Label, request.Required, request.Options, request.ShowInList),
                    cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Edit)
            .WithName("UpdateCustomField")
            .Produces<CustomFieldDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        fields.MapPut("/reorder", async (
                ReorderCustomFieldsRequest request, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new ReorderCustomFieldsCommand(request.EntityType, request.Ids), cancellationToken))
                .ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Edit)
            .WithName("ReorderCustomFields")
            .Produces<IReadOnlyList<CustomFieldDto>>()
            .ProducesValidationProblem();

        fields.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new DeleteCustomFieldCommand(id), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Settings, PermissionAction.Delete)
            .WithName("DeleteCustomField")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return api;
    }
}
