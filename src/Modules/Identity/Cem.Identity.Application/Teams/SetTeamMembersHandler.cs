using Cem.Identity.Application.Abstractions;
using Cem.Identity.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Teams;

public sealed class SetTeamMembersHandler(IIdentityStore store, IPermissionCacheInvalidator permissionCache)
    : ICommandHandler<SetTeamMembersCommand, Result>
{
    public async ValueTask<Result> Handle(SetTeamMembersCommand command, CancellationToken cancellationToken)
    {
        var team = await store.FindTeamAsync(command.TeamId, cancellationToken);
        if (team is null)
        {
            return Result.Failure(TeamErrors.NotFound);
        }

        var wanted = command.MembershipIds.Distinct().ToList();
        if (await store.CountActiveMembershipsAsync(wanted, cancellationToken) != wanted.Count)
        {
            return Result.Failure(Error.Validation(
                "teams.invalid_members",
                "Every member must be an active member of this organization.",
                new Dictionary<string, string[]> { ["membershipIds"] = ["Every member must be an active member of this organization."] }));
        }

        var current = await store.ListTeamMembersAsync(team.Id, cancellationToken);

        foreach (var member in current.Where(m => !wanted.Contains(m.MembershipId)))
        {
            store.Remove(member);
        }

        foreach (var membershipId in wanted.Where(id => current.All(m => m.MembershipId != id)))
        {
            store.Add(TeamMember.Create(team.Id, membershipId));
        }

        await store.SaveChangesAsync(cancellationToken);
        permissionCache.InvalidateTenant();
        return Result.Success();
    }
}
