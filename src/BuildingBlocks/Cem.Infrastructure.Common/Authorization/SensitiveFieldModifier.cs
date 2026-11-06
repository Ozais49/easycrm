using System.Text.Json.Serialization.Metadata;
using Cem.SharedKernel.Permissions;

namespace Cem.Infrastructure.Common.Authorization;

/// <summary>
/// System.Text.Json type-info modifier that enforces field-level security on properties tagged
/// <see cref="SensitiveAttribute"/>. Both directions fail closed: with no loaded permissions the caller has
/// no access to any group.
/// </summary>
public static class SensitiveFieldModifier
{
    /// <param name="currentPermissions">The permissions of the request being served, or null outside a request.</param>
    public static Action<JsonTypeInfo> Create(Func<IPermissionService?> currentPermissions) => typeInfo =>
    {
        var guarded = new List<(JsonPropertyInfo Property, FieldGroup Group)>();

        foreach (var property in typeInfo.Properties)
        {
            var sensitive = property.AttributeProvider?
                .GetCustomAttributes(typeof(SensitiveAttribute), inherit: true)
                .OfType<SensitiveAttribute>()
                .FirstOrDefault();

            if (sensitive is not null)
            {
                GuardRead(property, sensitive.Group, currentPermissions);
                GuardSetter(property, sensitive.Group, currentPermissions);
                guarded.Add((property, sensitive.Group));
            }
        }

        if (guarded.Count > 0)
        {
            typeInfo.OnDeserialized += instance => RejectConstructorBoundValues(instance, guarded, currentPermissions);
        }
    };

    // Absent from the response, not null, so clients cannot tell "hidden" from "empty" by shape.
    private static void GuardRead(JsonPropertyInfo property, FieldGroup group, Func<IPermissionService?> currentPermissions)
    {
        var shouldSerialize = property.ShouldSerialize;
        property.ShouldSerialize = (instance, value) =>
            currentPermissions() is { IsLoaded: true } permissions
            && permissions.CanReadField(group)
            && (shouldSerialize?.Invoke(instance, value) ?? true);
    }

    // Reject rather than silently ignore: dropping the value would lose the data of the caller without telling
    // them; an explicit 403 lets the client fix the request.
    private static void GuardSetter(JsonPropertyInfo property, FieldGroup group, Func<IPermissionService?> currentPermissions)
    {
        var set = property.Set;
        if (set is null)
        {
            return;
        }

        property.Set = (instance, value) =>
        {
            EnsureCanEdit(group, property.Name, currentPermissions);
            set(instance, value);
        };
    }

    // Properties bound through a constructor (positional records) never go through Set. After the object is built,
    // a value other than the default means the caller sent it; "not sent" is null for the nullable fields the
    // attribute asks for.
    private static void RejectConstructorBoundValues(
        object instance,
        List<(JsonPropertyInfo Property, FieldGroup Group)> guarded,
        Func<IPermissionService?> currentPermissions)
    {
        foreach (var (property, group) in guarded)
        {
            if (!IsDefault(property.Get?.Invoke(instance), property.PropertyType))
            {
                EnsureCanEdit(group, property.Name, currentPermissions);
            }
        }
    }

    private static bool IsDefault(object? value, Type type) =>
        value is null || (type.IsValueType && value.Equals(Activator.CreateInstance(type)));

    private static void EnsureCanEdit(FieldGroup group, string propertyName, Func<IPermissionService?> currentPermissions)
    {
        if (currentPermissions() is not { IsLoaded: true } permissions || !permissions.CanEditField(group))
        {
            throw new SensitiveFieldWriteException(group, propertyName);
        }
    }
}
