using Cem.Infrastructure.Common.Http;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cem.Infrastructure.Common.Authorization;

/// <summary>Rejects the request with 403 <c>permission.denied</c> unless the user's level for the action is above none.</summary>
public sealed class RequirePermissionFilter(Scope scope, PermissionAction action) : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Throws if permissions were not loaded: an endpoint outside the tenant group fails closed (500), never open.
        var permissions = context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();

        return permissions.Can(scope, action)
            ? next(context)
            : ValueTask.FromResult<object?>(
                Error.Forbidden("permission.denied", $"You need {scope}:{action} permission for this.").ToHttpResult());
    }
}
