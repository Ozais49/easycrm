namespace Cem.Identity.Application.Me;

/// <summary>The stored part of <c>/api/me</c>; permissions and teams come from the permission service.</summary>
public sealed record MeIdentity(MeUserDto User, MeTenantDto Tenant, IReadOnlyList<string> Roles);
