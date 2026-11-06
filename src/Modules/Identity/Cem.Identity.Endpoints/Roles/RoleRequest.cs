using Cem.SharedKernel.Permissions;

namespace Cem.Identity.Endpoints.Roles;

/// <summary>Body for creating and updating a role; permissions missing from the matrix mean none.</summary>
public sealed record RoleRequest(string Name, PermissionMatrix Permissions);
