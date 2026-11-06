using Cem.SharedKernel.Permissions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Cem.Infrastructure.Common.Authorization;

public static class RequirePermissionExtensions
{
    /// <summary>
    /// Requires the user to hold the permission. Use it on endpoints of the tenant group: the tenant middleware has
    /// then already loaded the permissions this filter reads (middleware runs before any endpoint filter).
    /// </summary>
    public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, Scope scope, PermissionAction action)
        where TBuilder : IEndpointConventionBuilder =>
        builder.AddEndpointFilter(new RequirePermissionFilter(scope, action));
}
