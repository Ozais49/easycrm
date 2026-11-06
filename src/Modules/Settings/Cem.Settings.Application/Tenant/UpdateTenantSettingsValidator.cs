using System.Text.RegularExpressions;
using FluentValidation;

namespace Cem.Settings.Application.Tenant;

public sealed partial class UpdateTenantSettingsValidator : AbstractValidator<UpdateTenantSettingsCommand>
{
    public UpdateTenantSettingsValidator()
    {
        RuleFor(c => c.TimeZone).NotEmpty().Must(IsIanaTimeZone)
            .WithMessage("Time zone must be an IANA time zone id, e.g. Asia/Kathmandu.");
        RuleFor(c => c.DateDisplay).IsInEnum();
        RuleFor(c => c.BrandColor).Matches("^#[0-9A-Fa-f]{6}$").When(c => c.BrandColor is not null)
            .WithMessage("Brand color must be a #RRGGBB hex color.");
    }

    // With InvariantGlobalization the Windows build cannot look up IANA ids at all (no ICU), so Windows dev machines
    // only get the shape check. On Linux (production) the tz database is read directly and the id must exist.
    private static bool IsIanaTimeZone(string id) =>
        IanaIdShape().IsMatch(id) && (OperatingSystem.IsWindows() || TimeZoneInfo.TryFindSystemTimeZoneById(id, out _));

    [GeneratedRegex("^(UTC|[A-Za-z]+(/[A-Za-z0-9_+-]+){1,2})$")]
    private static partial Regex IanaIdShape();
}
