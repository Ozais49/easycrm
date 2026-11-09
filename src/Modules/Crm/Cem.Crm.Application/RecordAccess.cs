using System.Linq.Expressions;
using Cem.Crm.Domain;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;

namespace Cem.Crm.Application;

/// <summary>
/// Which records the caller reaches for one action: all, those of the user teams (or owned), or only owned.
/// Used both as a list filter and as a check on a single loaded record.
/// </summary>
public sealed class RecordAccess
{
    /// <summary>No restriction; for re-reading a record the caller just wrote.</summary>
    public static RecordAccess Unrestricted { get; } = new(PermissionLevel.All, Guid.Empty, []);

    private readonly PermissionLevel _level;
    private readonly Guid _userId;
    private readonly Guid[] _teamIds;

    private RecordAccess(PermissionLevel level, Guid userId, Guid[] teamIds)
    {
        _level = level;
        _userId = userId;
        _teamIds = teamIds;
    }

    public static RecordAccess For(Scope scope, PermissionAction action, IPermissionService permissions, ICurrentUser user) =>
        new(permissions.LevelFor(scope, action), user.UserId ?? Guid.Empty, [.. permissions.TeamIds]);

    public bool Allows(IOwnedRecord record) => _level switch
    {
        PermissionLevel.All => true,
        PermissionLevel.Team => record.OwnerUserId == _userId || (record.TeamId is { } team && _teamIds.Contains(team)),
        PermissionLevel.Own => record.OwnerUserId == _userId,
        _ => false
    };

    /// <summary>The same rule as <see cref="Allows"/>, as a query predicate.</summary>
    public Expression<Func<T, bool>> Filter<T>() where T : IOwnedRecord
    {
        var userId = _userId;
        var teamIds = _teamIds;

        return _level switch
        {
            PermissionLevel.All => e => true,
            PermissionLevel.Team => e => e.OwnerUserId == userId || (e.TeamId != null && teamIds.Contains(e.TeamId.Value)),
            PermissionLevel.Own => e => e.OwnerUserId == userId,
            _ => e => false
        };
    }
}
