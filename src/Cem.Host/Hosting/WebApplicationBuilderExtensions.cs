using System.Text.Json;
using System.Text.Json.Serialization;
using Cem.Identity.Endpoints;
using Cem.Identity.Infrastructure;
using Cem.Infrastructure.Common.Auth;
using Cem.Infrastructure.Common.Authorization;
using Cem.Infrastructure.Common.Behaviors;
using Cem.Infrastructure.Common.Tenancy;

namespace Cem.Host.Hosting;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddCemHost(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddSingleton(TimeProvider.System);
        services.AddProblemDetails();
        services.AddExceptionHandler<SensitiveFieldExceptionHandler>();
        services.AddOpenApi();
        services.AddHealthChecks();

        // Bounded: holds resolved sessions only (see TenantResolutionFilter).
        services.AddMemoryCache(cache => cache.SizeLimit = 1000);

        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(cors => cors.AddDefaultPolicy(policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()));

        services.AddClerkAuthentication(builder.Configuration, builder.Environment);
        services.AddRequestContext();

        // Enum values travel as camelCase text (roles-and-permissions: "all", "contactPii"), also as dictionary keys.
        services.ConfigureHttpJsonOptions(json =>
        {
            json.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            json.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        });
        services.AddSensitiveFieldSecurity();

        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
        });

        services.AddIdentityModule(builder.Configuration);
        services.AddIdentityEndpoints();

        return builder;
    }
}
