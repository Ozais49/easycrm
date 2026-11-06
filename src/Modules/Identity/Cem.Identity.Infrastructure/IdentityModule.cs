using Cem.Identity.Application.Abstractions;
using Cem.Identity.Infrastructure.Persistence;
using Cem.Infrastructure.Common.Persistence;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cem.Identity.Infrastructure;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Cem")
            ?? throw new InvalidOperationException("Connection string 'Cem' is missing.");

        services.AddCemDbContext<IdentityDbContext>(connectionString, IdentityDbContext.SchemaName);
        services.AddScoped<IIdentityStore, IdentityStore>();
        services.AddScoped<IdentityProvisioner>();
        services.AddValidatorsFromAssemblyContaining<IdentityProvisioner>(ServiceLifetime.Singleton);

        return services;
    }
}
