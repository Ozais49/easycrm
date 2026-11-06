using Cem.Identity.Application.Me;
using Cem.Infrastructure.Common.Http;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cem.Identity.Endpoints.Me;

public static class MeEndpoints
{
    /// <summary>Open to every authenticated member: the UI needs it to know what to show.</summary>
    public static IEndpointRouteBuilder MapMe(this IEndpointRouteBuilder api)
    {
        api.MapGet("/me", async (ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetMeQuery(), cancellationToken)).ToHttpResult())
            .WithName("GetMe")
            .Produces<MeDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return api;
    }
}
