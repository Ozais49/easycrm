using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Teams;

public sealed class RenameTeamHandler(IIdentityStore store) : ICommandHandler<RenameTeamCommand, Result>
{
    public async ValueTask<Result> Handle(RenameTeamCommand command, CancellationToken cancellationToken)
    {
        var team = await store.FindTeamAsync(command.TeamId, cancellationToken);
        if (team is null)
        {
            return Result.Failure(TeamErrors.NotFound);
        }

        var name = command.Name.Trim();
        if (await store.TeamNameTakenAsync(name, team.Id, cancellationToken))
        {
            return Result.Failure(TeamErrors.NameTaken);
        }

        team.Rename(name);
        if (!await store.TrySaveNewAsync(cancellationToken))
        {
            return Result.Failure(TeamErrors.NameTaken);
        }

        return Result.Success();
    }
}
