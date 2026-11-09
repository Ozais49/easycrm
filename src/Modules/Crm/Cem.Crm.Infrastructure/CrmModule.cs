using Cem.Crm.Application;
using Cem.Crm.Application.Abstractions;
using Cem.Crm.Infrastructure.Persistence;
using Cem.Infrastructure.Common.Persistence;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cem.Crm.Infrastructure;

public static class CrmModule
{
    public static IServiceCollection AddCrmModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Cem")
            ?? throw new InvalidOperationException("Connection string 'Cem' is missing.");

        services.AddCemDbContext<CrmDbContext>(connectionString, CrmDbContext.SchemaName);
        services.AddScoped<ICrmStore, CrmStore>();
        services.AddScoped<ICrmQueries, CrmQueries>();
        services.AddValidatorsFromAssemblyContaining<AddressValidator>(ServiceLifetime.Singleton);

        return services;
    }
}
