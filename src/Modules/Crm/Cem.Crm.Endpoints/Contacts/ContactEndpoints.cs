using Cem.Crm.Application.Contacts;
using Cem.Infrastructure.Common.Authorization;
using Cem.Infrastructure.Common.Http;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cem.Crm.Endpoints.Contacts;

public static class ContactEndpoints
{
    public static IEndpointRouteBuilder MapContacts(this IEndpointRouteBuilder api)
    {
        var contacts = api.MapGroup("/contacts");

        contacts.MapGet("/", async (
                ISender sender, CancellationToken cancellationToken,
                int page = 1, int pageSize = 25, string? q = null, Guid? companyId = null) =>
                (await sender.Send(new ListContactsQuery(page, pageSize, q, companyId), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Crm, PermissionAction.Read)
            .WithName("ListContacts")
            .Produces<PagedResult<ContactDto>>()
            .ProducesValidationProblem();

        contacts.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new GetContactQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Crm, PermissionAction.Read)
            .WithName("GetContact")
            .Produces<ContactDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        contacts.MapPost("/", async (ContactRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new CreateContactCommand(
                        request.FirstName, request.LastName, request.JobTitle, request.CompanyId, request.Email,
                        request.Phone, request.Mobile, request.Address, request.OwnerUserId, request.TeamId),
                    cancellationToken);
                return result.IsSuccess
                    ? TypedResults.Created($"/api/contacts/{result.Value.Id}", result.Value)
                    : result.Error.ToHttpResult();
            })
            .RequirePermission(Scope.Crm, PermissionAction.Create)
            .WithName("CreateContact")
            .Produces<ContactDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        contacts.MapPut("/{id:guid}", async (
                Guid id, ContactRequest request, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(
                    new UpdateContactCommand(
                        id, request.FirstName, request.LastName, request.JobTitle, request.CompanyId, request.Email,
                        request.Phone, request.Mobile, request.Address, request.OwnerUserId, request.TeamId),
                    cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Crm, PermissionAction.Edit)
            .WithName("UpdateContact")
            .Produces<ContactDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        contacts.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new DeleteContactCommand(id), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Crm, PermissionAction.Delete)
            .WithName("DeleteContact")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return api;
    }
}
