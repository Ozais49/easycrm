namespace Cem.SharedKernel.Permissions;

/// <summary>
/// Marks a DTO property as part of a <see cref="FieldGroup"/>. Responses omit it for callers who cannot read the
/// group; requests that set it are rejected for callers who cannot edit the group.
/// </summary>
/// <remarks>
/// Sensitive request fields should be nullable (<c>decimal?</c>, <c>string?</c>) so that "not sent" is null:
/// for properties bound through a constructor (positional records), any value other than the type default counts
/// as sent. A non-nullable value type sent as its default (e.g. <c>0</c>) is not detected, which is harmless
/// because it is indistinguishable from not sent.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SensitiveAttribute(FieldGroup group) : Attribute
{
    public FieldGroup Group { get; } = group;
}
