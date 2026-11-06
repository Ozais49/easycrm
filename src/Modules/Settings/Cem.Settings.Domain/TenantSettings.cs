using Cem.SharedKernel;

namespace Cem.Settings.Domain;

/// <summary>Regional and branding preferences of a tenant; exactly one row per tenant.</summary>
public sealed class TenantSettings : AuditableEntity, ITenantEntity
{
    private TenantSettings()
    {
    }

    public Guid TenantId { get; private set; }

    /// <summary>IANA time zone id, e.g. <c>Asia/Kathmandu</c>.</summary>
    public string TimeZone { get; private set; } = "";

    public DateDisplay DateDisplay { get; private set; }

    /// <summary><c>#RRGGBB</c>; null means the default accent of the app.</summary>
    public string? BrandColor { get; private set; }

    public static TenantSettings CreateDefault() => new()
    {
        TimeZone = "Asia/Kathmandu",
        DateDisplay = DateDisplay.Both
    };

    public void Update(string timeZone, DateDisplay dateDisplay, string? brandColor)
    {
        TimeZone = timeZone;
        DateDisplay = dateDisplay;
        BrandColor = brandColor;
    }
}
