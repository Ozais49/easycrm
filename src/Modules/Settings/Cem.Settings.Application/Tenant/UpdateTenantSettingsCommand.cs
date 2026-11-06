using Cem.Settings.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.Tenant;

public sealed record UpdateTenantSettingsCommand(string TimeZone, DateDisplay DateDisplay, string? BrandColor)
    : ICommand<Result<TenantSettingsDto>>;
