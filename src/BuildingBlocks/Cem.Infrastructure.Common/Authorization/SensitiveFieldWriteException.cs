using Cem.SharedKernel.Permissions;

namespace Cem.Infrastructure.Common.Authorization;

/// <summary>A request body set a <see cref="SensitiveAttribute"/> property the caller is not allowed to edit.</summary>
public sealed class SensitiveFieldWriteException(FieldGroup group, string propertyName)
    : Exception($"You are not allowed to set '{propertyName}' ({group}).")
{
    public FieldGroup Group { get; } = group;

    public string PropertyName { get; } = propertyName;
}
