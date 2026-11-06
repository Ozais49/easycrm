using System.Security.Claims;
using System.Text.Json;

namespace Cem.Identity.Endpoints.Tenancy;

/// <summary>User and active-organization identifiers read from a Clerk session token (v1 and v2 formats).</summary>
/// <param name="OrgRole">Clerk organization role: <c>admin</c> in v2 tokens, <c>org:admin</c> in v1 tokens.</param>
public sealed record ClerkClaims(string? UserId, string? OrgId, string? OrgSlug, string? OrgRole)
{
    public bool IsOrgAdmin => OrgRole is "admin" or "org:admin";

    public static ClerkClaims From(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue("sub");

        // v2 tokens: "o" is a JSON object {"id": "org_...", "rol": "...", "slg": "..."}.
        if (principal.FindFirstValue("o") is { } organization && TryReadV2(organization, out var orgId, out var slug, out var role))
        {
            return new ClerkClaims(userId, orgId, slug, role);
        }

        // v1 tokens: flat org_id / org_slug / org_role claims.
        return new ClerkClaims(
            userId,
            principal.FindFirstValue("org_id"),
            principal.FindFirstValue("org_slug"),
            principal.FindFirstValue("org_role"));
    }

    private static bool TryReadV2(string json, out string? orgId, out string? slug, out string? role)
    {
        orgId = null;
        slug = null;
        role = null;

        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            orgId = ReadString(document.RootElement, "id");
            slug = ReadString(document.RootElement, "slg");
            role = ReadString(document.RootElement, "rol");
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
