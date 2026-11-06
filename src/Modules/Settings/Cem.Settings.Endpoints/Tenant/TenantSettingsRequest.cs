using Cem.Settings.Domain;

namespace Cem.Settings.Endpoints.Tenant;

/// <summary>Body for saving the tenant settings.</summary>
public sealed record TenantSettingsRequest(string TimeZone, DateDisplay DateDisplay, string? BrandColor);
