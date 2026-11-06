namespace Cem.SharedKernel.Permissions;

public readonly record struct FieldAccess(bool Read, bool Edit)
{
    public static FieldAccess None => new(false, false);
    public static FieldAccess ReadOnly => new(true, false);
    public static FieldAccess ReadWrite => new(true, true);
}
