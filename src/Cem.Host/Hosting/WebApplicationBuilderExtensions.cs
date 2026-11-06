using Cem.Infrastructure.Common.Behaviors;

namespace Cem.Host.Hosting;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddCemHost(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddSingleton(TimeProvider.System);
        services.AddProblemDetails();
        services.AddOpenApi();
        services.AddHealthChecks();

        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(cors => cors.AddDefaultPolicy(policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()));

        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
        });

        // Modules register here, e.g. services.AddCrmModule(builder.Configuration);

        return builder;
    }
}
