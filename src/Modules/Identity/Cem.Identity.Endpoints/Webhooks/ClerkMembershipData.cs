namespace Cem.Identity.Endpoints.Webhooks;

public sealed record ClerkMembershipData(string? Id, ClerkOrganizationData? Organization, ClerkPublicUserData? PublicUserData);
