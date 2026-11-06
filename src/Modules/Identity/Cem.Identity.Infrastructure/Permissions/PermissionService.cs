using Cem.Identity.Application.Abstractions;
using Cem.Identity.Application.Permissions;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;

namespace Cem.Identity.Infrastructure.Permissions;

public sealed class PermissionService(PermissionCache cache, IIdentityQueries queries, ICurrentUser currentUser)
    : IPermissionService
{
    private PermissionSnapshot? _snapshot;

    public bool IsLoaded => _snapshot is not null;

    public PermissionMatrix Matrix => Snapshot.Matrix;

    public IReadOnlyCollection<Guid> TeamIds => Snapshot.TeamIds;

    private PermissionSnapshot Snapshot =>
        _snapshot ?? throw new InvalidOperationException("Permissions are not loaded: the tenant filter must run first.");

    public async ValueTask LoadAsync(CancellationToken cancellationToken) =>
        _snapshot ??= await cache.GetOrLoadAsync(
            () => queries.GetPermissionSnapshotAsync(currentUser.UserId!.Value, cancellationToken));

    public PermissionLevel LevelFor(Scope scope, PermissionAction action) => Matrix.LevelFor(scope, action);

    public bool Can(Scope scope, PermissionAction action) => LevelFor(scope, action) > PermissionLevel.None;

    public bool CanReadField(FieldGroup group) => Matrix.CanReadField(group);

    public bool CanEditField(FieldGroup group) => Matrix.CanEditField(group);
}
