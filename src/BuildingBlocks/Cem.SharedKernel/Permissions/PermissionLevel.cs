namespace Cem.SharedKernel.Permissions;

/// <summary>How far a permission reaches. Ordered by reach, so the union of several roles is the maximum.</summary>
public enum PermissionLevel
{
    None,
    Own,
    Team,
    All
}
