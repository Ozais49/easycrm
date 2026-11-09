using Cem.Crm.Application.Companies;
using Cem.Infrastructure.Common.Authorization;
using Cem.Infrastructure.Common.Http;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cem.Crm.Endpoints.Companies;

public static class CompanyEndpoints
{
    public static IEndpointRouteBuilder MapCompanies(this IEndpointRouteBuilder api)
    {
        var companies = api.MapGroup("/companies");

        companies.MapGet("/", async (
                ISender sender, CancellationToken cancellationToken, int page = 1, int pageSize = 25, string? q = null,
                Guid? tagId = null) =>
                (await sender.Send(new ListCompaniesQuery(page, pageSize, q, tagId), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Crm, PermissionAction.Read)
            .WithName("ListCompanies")
            .Produces<PagedResult<CompanyDto>>()
            .ProducesValidationProblem();

        companies.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new GetCompanyQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Crm, PermissionAction.Read)
            .WithName("GetCompany")
            .Produces<CompanyDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        companies.MapPost("/", async (CompanyRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new CreateCompanyCommand(
                        request.Name, request.Industry, request.Website, request.Phone, request.Email,
                        request.PanNumber, request.Address, request.OwnerUserId, request.TeamId, request.Tags, request.CustomFields),
                    cancellationToken);
                return result.IsSuccess
                    ? TypedResults.Created($"/api/companies/{result.Value.Id}", result.Value)
                    : result.Error.ToHttpResult();
            })
            .RequirePermission(Scope.Crm, PermissionAction.Create)
            .WithName("CreateCompany")
            .Produces<CompanyDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        companies.MapPut("/{id:guid}", async (
                Guid id, CompanyRequest request, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(
                    new UpdateCompanyCommand(
                        id, request.Name, request.Industry, request.Website, request.Phone, request.Email,
                        request.PanNumber, request.Address, request.OwnerUserId, request.TeamId, request.Tags, request.CustomFields),
                    cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Crm, PermissionAction.Edit)
            .WithName("UpdateCompany")
            .Produces<CompanyDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // Take up an unassigned company. Any edit level will do, "own" included (see ClaimCompanyHandler).
        companies.MapPost("/{id:guid}/claim", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new ClaimCompanyCommand(id), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Crm, PermissionAction.Edit)
            .WithName("ClaimCompany")
            .Produces<CompanyDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        companies.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new DeleteCompanyCommand(id), cancellationToken)).ToHttpResult())
            .RequirePermission(Scope.Crm, PermissionAction.Delete)
            .WithName("DeleteCompany")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return api;
    }
}
