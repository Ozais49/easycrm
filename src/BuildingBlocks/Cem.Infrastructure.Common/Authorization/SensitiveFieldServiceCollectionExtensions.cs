using Cem.SharedKernel.Permissions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization.Metadata;

namespace Cem.Infrastructure.Common.Authorization;

public static class SensitiveFieldServiceCollectionExtensions
{
    /// <summary>Applies field-level security to everything the minimal-API JSON options (de)serialize.</summary>
    public static IServiceCollection AddSensitiveFieldSecurity(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddOptions<JsonOptions>()
            .Configure<IHttpContextAccessor>((json, accessor) =>
            {
                var modifier = SensitiveFieldModifier.Create(() =>
                    accessor.HttpContext?.RequestServices.GetService<IPermissionService>());

                var options = json.SerializerOptions;
                options.TypeInfoResolver = (options.TypeInfoResolver ?? new DefaultJsonTypeInfoResolver())
                    .WithAddedModifier(modifier);
            });

        return services;
    }
}
