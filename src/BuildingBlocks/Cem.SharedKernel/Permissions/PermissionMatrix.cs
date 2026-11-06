using System.Text.Json.Serialization;

namespace Cem.SharedKernel.Permissions;

/// <summary>
/// Immutable permission grid: level per scope and action, read/edit per field group. Always complete:
/// anything missing from the input means none. Its JSON form is the one stored on a role and returned by the API.
/// </summary>
public sealed class PermissionMatrix
{
    public static PermissionMatrix Empty { get; } = new(null, null);

    [JsonConstructor]
    public PermissionMatrix(
        IReadOnlyDictionary<Scope, IReadOnlyDictionary<PermissionAction, PermissionLevel>>? scopes,
        IReadOnlyDictionary<FieldGroup, FieldAccess>? fields)
    {
        Scopes = Enum.GetValues<Scope>().ToDictionary(
            scope => scope,
            scope => (IReadOnlyDictionary<PermissionAction, PermissionLevel>)Enum.GetValues<PermissionAction>().ToDictionary(
                action => action,
                action => scopes?.GetValueOrDefault(scope)?.GetValueOrDefault(action) ?? PermissionLevel.None));

        Fields = Enum.GetValues<FieldGroup>().ToDictionary(
            group => group,
            group => fields?.GetValueOrDefault(group) ?? FieldAccess.None);
    }

    public IReadOnlyDictionary<Scope, IReadOnlyDictionary<PermissionAction, PermissionLevel>> Scopes { get; }

    public IReadOnlyDictionary<FieldGroup, FieldAccess> Fields { get; }

    public PermissionLevel LevelFor(Scope scope, PermissionAction action) => Scopes[scope][action];

    public bool CanReadField(FieldGroup group) => Fields[group].Read;

    public bool CanEditField(FieldGroup group) => Fields[group].Edit;

    /// <summary>Highest level per scope and action, and read/edit if any matrix allows it.</summary>
    public static PermissionMatrix Union(IEnumerable<PermissionMatrix> matrices)
    {
        var all = matrices.ToList();

        return new PermissionMatrix(
            Enum.GetValues<Scope>().ToDictionary(
                scope => scope,
                scope => (IReadOnlyDictionary<PermissionAction, PermissionLevel>)Enum.GetValues<PermissionAction>().ToDictionary(
                    action => action,
                    action => all.Select(m => m.LevelFor(scope, action)).DefaultIfEmpty().Max())),
            Enum.GetValues<FieldGroup>().ToDictionary(
                group => group,
                group => new FieldAccess(all.Any(m => m.CanReadField(group)), all.Any(m => m.CanEditField(group)))));
    }
}
