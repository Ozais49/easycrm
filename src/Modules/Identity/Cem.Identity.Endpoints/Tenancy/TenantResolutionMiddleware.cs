using Cem.Identity.Application.Sessions;
using Cem.Infrastructure.Common.Http;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Cem.Identity.Endpoints.Tenancy;

/// <summary>
/// Turns the authenticated Clerk session into the request tenant and user, provisioning them on first sight,
/// then loads the permissions of the user. Resolved sessions are cached briefly so most requests skip the database.
/// </summary>
/// <remarks>
/// Middleware rather than an endpoint filter on purpose: minimal APIs bind the request body before endpoint filters
/// run, and field-level security needs the permissions while the body is being deserialized.
/// </remarks>
public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    // Short on purpose: a removed member keeps access for at most this long (the Clerk token lives 60 s anyway).
    private static readonly TimeSpan SessionTtl = TimeSpan.FromSeconds(60);

    public async Task InvokeAsync(
        HttpContext http,
        IMediator mediator,
        IMemoryCache cache,
        IRequestContextSetter requestContext,
        IPermissionService permissions)
    {
        if (http.GetEndpoint()?.Metadata.GetMetadata<TenantRequiredMetadata>() is null)
        {
            await next(http);
            return;
        }

        var claims = ClerkClaims.From(http.User);

        if (claims.UserId is null)
        {
            await Error.Unauthorized("auth.invalid_token", "The token has no subject.").ToHttpResult().ExecuteAsync(http);
            return;
        }

        if (claims.OrgId is null)
        {
            await Error.Forbidden("tenant.required", "Select an organization to continue.").ToHttpResult().ExecuteAsync(http);
            return;
        }

        var cacheKey = $"session:{claims.OrgId}:{claims.UserId}";
        if (!cache.TryGetValue(cacheKey, out SessionInfo? session) || session is null)
        {
            var result = await mediator.Send(
                new ResolveSessionCommand(claims.UserId, claims.OrgId, claims.OrgSlug, claims.IsOrgAdmin),
                http.RequestAborted);

            if (result.IsFailure)
            {
                await result.Error.ToHttpResult().ExecuteAsync(http);
                return;
            }

            session = result.Value;
            cache.Set(cacheKey, session, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = SessionTtl, Size = 1 });
        }

        requestContext.SetTenant(session.TenantId);
        requestContext.SetUser(session.UserId, claims.UserId);
        await permissions.LoadAsync(http.RequestAborted);

        await next(http);
    }
}
