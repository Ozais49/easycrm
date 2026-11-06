using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Cem.Infrastructure.Common.Auth;

public static class ClerkAuthenticationExtensions
{
    // Keeps the JwtBearer scheme constructible while Clerk is unconfigured; tokens then fail discovery and get a 401.
    private const string UnconfiguredAuthority = "https://clerk-not-configured.invalid";

    /// <summary>
    /// Clerk session-token authentication; every endpoint requires an authenticated user unless it opts out
    /// with <c>AllowAnonymous</c>.
    /// </summary>
    /// <remarks>
    /// Outside Development, missing Clerk settings stop the app at startup. In Development they are optional so the
    /// app (and <c>/health</c>) runs before Clerk is set up; authenticated calls then simply fail with 401.
    /// </remarks>
    public static IServiceCollection AddClerkAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var options = services.AddOptions<ClerkOptions>().Bind(configuration.GetSection(ClerkOptions.SectionName));
        if (!environment.IsDevelopment())
        {
            options
                .Validate(o => o.IsComplete, "Clerk:Authority, Clerk:AuthorizedParties and Clerk:WebhookSigningSecret are required.")
                .ValidateOnStart();
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<ClerkOptions>>((jwt, clerkOptions) =>
            {
                var clerk = clerkOptions.Value;

                jwt.Authority = clerk.Authority.Length > 0 ? clerk.Authority : UnconfiguredAuthority;
                jwt.MapInboundClaims = false; // keep Clerk's claim names (sub, o, org_id) as they are
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, // issuer comes from the Authority's discovery document
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    // Clerk session tokens carry no "aud" by default; the authorized-parties check below replaces it.
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.FromSeconds(5)
                };
                jwt.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Clerk recommends checking "azp" (the origin that requested the token) when present.
                        var azp = context.Principal?.FindFirst("azp")?.Value;
                        if (azp is not null && !clerk.AuthorizedParties.Contains(azp))
                        {
                            context.Fail("Token was issued to an unauthorized party.");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        return services;
    }
}
