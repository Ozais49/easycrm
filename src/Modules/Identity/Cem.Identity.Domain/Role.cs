using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;

namespace Cem.Identity.Domain;

/// <summary>A named set of permissions. The system role (Admin) is protected so a tenant can never lock itself out.</summary>
public sealed class Role : AuditableEntity, ITenantEntity
{
    private Role()
    {
    }

    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = "";
    public bool IsSystem { get; private set; }
    public PermissionMatrix Permissions { get; private set; } = PermissionMatrix.Empty;

    public static Role Create(string name, PermissionMatrix permissions) =>
        new() { Name = name, Permissions = permissions };

    public static Role CreateSystem(string name, PermissionMatrix permissions) =>
        new() { Name = name, Permissions = permissions, IsSystem = true };

    public Result Update(string name, PermissionMatrix permissions)
    {
        if (IsSystem && name != Name)
        {
            return Result.Failure(Error.Conflict("roles.system_role", $"The {Name} role cannot be renamed."));
        }

        if (IsSystem && !KeepsFullSettingsAccess(permissions))
        {
            return Result.Failure(Error.Validation(
                "roles.settings_required",
                $"The {Name} role must keep full access to Settings."));
        }

        Name = name;
        Permissions = permissions;
        return Result.Success();
    }

    public Result EnsureCanBeDeleted() =>
        IsSystem
            ? Result.Failure(Error.Conflict("roles.system_role", $"The {Name} role cannot be deleted."))
            : Result.Success();

    private static bool KeepsFullSettingsAccess(PermissionMatrix permissions) =>
        Enum.GetValues<PermissionAction>().All(action => permissions.LevelFor(Scope.Settings, action) == PermissionLevel.All);
}
