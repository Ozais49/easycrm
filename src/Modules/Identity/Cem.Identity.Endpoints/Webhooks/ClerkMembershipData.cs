namespace Cem.Identity.Endpoints.Webhooks;

/// <param name="Role">Clerk organization role, e.g. <c>org:admin</c>.</param>
public sealed record ClerkMembershipData(string? Id, ClerkOrganizationData? Organization, ClerkPublicUserData? PublicUserData, string? Role);
