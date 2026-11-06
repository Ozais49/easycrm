using Cem.SharedKernel;

namespace Cem.Identity.Application.Roles;

internal static class RoleErrors
{
    public static readonly Error NotFound = Error.NotFound("roles.not_found", "Role not found.");

    public static readonly Error NameTaken = Error.Conflict("roles.name_taken", "Another role already has this name.");
}
