using Cem.Identity.Contracts;
using Cem.Infrastructure.Common.Outbox;
using Cem.Infrastructure.Common.Persistence;
using Cem.Settings.Application.Abstractions;
using Cem.Settings.Application.Provisioning;
using Cem.Settings.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cem.Settings.Infrastructure;

public static class SettingsModule
{
    public static IServiceCollection AddSettingsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Cem")
            ?? throw new InvalidOperationException("Connection string 'Cem' is missing.");

        services.AddCemDbContext<SettingsDbContext>(connectionString, SettingsDbContext.SchemaName);
        services.AddScoped<ISettingsStore, SettingsStore>();
        services.AddIntegrationEventHandler<TenantCreated, TenantCreatedHandler>();
        services.AddValidatorsFromAssemblyContaining<TenantCreatedHandler>(ServiceLifetime.Singleton);

        return services;
    }
}
