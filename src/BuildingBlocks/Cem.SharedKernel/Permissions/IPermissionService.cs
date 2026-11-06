namespace Cem.SharedKernel.Permissions;

/// <summary>
/// The effective permissions of the current user in the current tenant. Loaded once per request by the tenant
/// filter (<see cref="LoadAsync"/>); everything else reads it synchronously, which is what lets the JSON
/// serializer consult it. Record-level <see cref="PermissionLevel.Own"/> / <see cref="PermissionLevel.Team"/>
/// checks are the handler's job: compare the record's owner or team with the user and <see cref="TeamIds"/>.
/// </summary>
public interface IPermissionService
{
    bool IsLoaded { get; }

    /// <summary>Loads the permissions if that has not happened yet in this scope.</summary>
    ValueTask LoadAsync(CancellationToken cancellationToken);

    /// <summary>The union of the user's roles.</summary>
    PermissionMatrix Matrix { get; }

    /// <summary>Teams the user belongs to, for building "team" filters.</summary>
    IReadOnlyCollection<Guid> TeamIds { get; }

    PermissionLevel LevelFor(Scope scope, PermissionAction action);

    /// <summary>True when the level is anything above <see cref="PermissionLevel.None"/>.</summary>
    bool Can(Scope scope, PermissionAction action);

    bool CanReadField(FieldGroup group);

    bool CanEditField(FieldGroup group);
}
