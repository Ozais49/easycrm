using System.Security.Claims;
using System.Text.Json;

namespace Cem.Identity.Endpoints.Tenancy;

/// <summary>User and active-organization identifiers read from a Clerk session token (v1 and v2 formats).</summary>
public sealed record ClerkClaims(string? UserId, string? OrgId, string? OrgSlug)
{
    public static ClerkClaims From(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue("sub");

        // v2 tokens: "o" is a JSON object {"id": "org_...", "rol": "...", "slg": "..."}.
        if (principal.FindFirstValue("o") is { } organization && TryReadV2(organization, out var orgId, out var slug))
        {
            return new ClerkClaims(userId, orgId, slug);
        }

        // v1 tokens: flat org_id / org_slug claims.
        return new ClerkClaims(userId, principal.FindFirstValue("org_id"), principal.FindFirstValue("org_slug"));
    }

    private static bool TryReadV2(string json, out string? orgId, out string? slug)
    {
        orgId = null;
        slug = null;

        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            orgId = ReadString(document.RootElement, "id");
            slug = ReadString(document.RootElement, "slg");
            return orgId is not null;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static string? ReadString(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
}
