using Cem.Identity.Application.Sessions;
using Cem.Infrastructure.Common.Http;
using Cem.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Cem.Identity.Endpoints.Tenancy;

/// <summary>
/// Turns the authenticated Clerk session into the request's tenant and user, provisioning them on first sight.
/// Resolved sessions are cached briefly so most requests skip the database.
/// </summary>
public sealed class TenantResolutionFilter(IMediator mediator, IMemoryCache cache, IRequestContextSetter requestContext)
    : IEndpointFilter
{
    // Short on purpose: a removed member keeps access for at most this long (the Clerk token lives 60 s anyway).
    private static readonly TimeSpan SessionTtl = TimeSpan.FromSeconds(60);

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var http = context.HttpContext;
        var claims = ClerkClaims.From(http.User);

        if (claims.UserId is null)
        {
            return Error.Unauthorized("auth.invalid_token", "The token has no subject.").ToHttpResult();
        }

        if (claims.OrgId is null)
        {
            return Error.Forbidden("tenant.required", "Select an organization to continue.").ToHttpResult();
        }

        var cacheKey = $"session:{claims.OrgId}:{claims.UserId}";
        if (!cache.TryGetValue(cacheKey, out SessionInfo? session) || session is null)
        {
            var result = await mediator.Send(
                new ResolveSessionCommand(claims.UserId, claims.OrgId, claims.OrgSlug),
                http.RequestAborted);

            if (result.IsFailure)
            {
                return result.Error.ToHttpResult();
            }

            session = result.Value;
            cache.Set(cacheKey, session, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = SessionTtl, Size = 1 });
        }

        requestContext.SetTenant(session.TenantId);
        requestContext.SetUser(session.UserId, claims.UserId);

        return await next(context);
    }
}
