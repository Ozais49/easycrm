using System.Linq.Expressions;
using Cem.Crm.Domain;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;

namespace Cem.Crm.Application;

/// <summary>
/// Which records the caller reaches for one action: all, those of the user teams (or owned), or only owned.
/// Used both as a list filter and as a check on a single loaded record.
/// <para>
/// Unassigned records (no owner) form a pool that people can take up. For read and create, the pool is reachable
/// at <c>own</c> and <c>team</c> level too: an unassigned record with a team by the members of that team, one
/// without a team by everyone. Edit and delete are not widened: a record is taken up first (claim), then edited.
/// </para>
/// </summary>
public sealed class RecordAccess
{
    /// <summary>No restriction; for re-reading a record the caller just wrote.</summary>
    public static RecordAccess Unrestricted { get; } = new(PermissionLevel.All, Guid.Empty, [], includePool: true);

    private readonly PermissionLevel _level;
    private readonly Guid _userId;
    private readonly Guid[] _teamIds;
    private readonly bool _includePool;

    private RecordAccess(PermissionLevel level, Guid userId, Guid[] teamIds, bool includePool)
    {
        _level = level;
        _userId = userId;
        _teamIds = teamIds;
        _includePool = includePool;
    }

    public static RecordAccess For(Scope scope, PermissionAction action, IPermissionService permissions, ICurrentUser user) =>
        new(
            permissions.LevelFor(scope, action),
            user.UserId ?? Guid.Empty,
            [.. permissions.TeamIds],
            includePool: action is PermissionAction.Read or PermissionAction.Create);

    public bool Allows(IOwnedRecord record) => Allows(record.OwnerUserId, record.TeamId);

    /// <summary>The same rule for an owner and team that are not saved yet, e.g. on create.</summary>
    public bool Allows(Guid? ownerUserId, Guid? teamId)
    {
        var inMyTeam = teamId is { } team && _teamIds.Contains(team);
        var inPool = _includePool && ownerUserId is null && (teamId is null || inMyTeam);

        return _level switch
        {
            PermissionLevel.All => true,
            PermissionLevel.Team => ownerUserId == _userId || inMyTeam || inPool,
            PermissionLevel.Own => ownerUserId == _userId || inPool,
            _ => false
        };
    }

    /// <summary>The same rule as <see cref="Allows(Guid?, Guid?)"/>, as a query predicate.</summary>
    public Expression<Func<T, bool>> Filter<T>() where T : IOwnedRecord
    {
        var userId = _userId;
        var teamIds = _teamIds;

        if (_level == PermissionLevel.All)
        {
            return e => true;
        }

        if (_level is not (PermissionLevel.Team or PermissionLevel.Own))
        {
            return e => false;
        }

        if (!_includePool)
        {
            return _level == PermissionLevel.Team
                ? e => e.OwnerUserId == userId || (e.TeamId != null && teamIds.Contains(e.TeamId.Value))
                : e => e.OwnerUserId == userId;
        }

        return _level == PermissionLevel.Team
            ? e => e.OwnerUserId == userId
                || (e.TeamId != null && teamIds.Contains(e.TeamId.Value))
                || (e.OwnerUserId == null && e.TeamId == null)
            : e => e.OwnerUserId == userId
                || (e.OwnerUserId == null && (e.TeamId == null || teamIds.Contains(e.TeamId.Value)));
    }
}
