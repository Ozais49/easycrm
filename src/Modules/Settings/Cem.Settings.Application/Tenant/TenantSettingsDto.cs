using Cem.Settings.Domain;

namespace Cem.Settings.Application.Tenant;

public sealed record TenantSettingsDto(string TimeZone, DateDisplay DateDisplay, string? BrandColor)
{
    public static TenantSettingsDto From(TenantSettings settings) =>
        new(settings.TimeZone, settings.DateDisplay, settings.BrandColor);
}
