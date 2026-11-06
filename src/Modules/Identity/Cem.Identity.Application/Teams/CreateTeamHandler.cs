using Cem.Identity.Application.Abstractions;
using Cem.Identity.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Teams;

public sealed class CreateTeamHandler(IIdentityStore store) : ICommandHandler<CreateTeamCommand, Result<TeamDto>>
{
    public async ValueTask<Result<TeamDto>> Handle(CreateTeamCommand command, CancellationToken cancellationToken)
    {
        var name = command.Name.Trim();
        if (await store.TeamNameTakenAsync(name, exceptTeamId: null, cancellationToken))
        {
            return Result<TeamDto>.Failure(TeamErrors.NameTaken);
        }

        var team = Team.Create(name);
        store.Add(team);
        if (!await store.TrySaveNewAsync(cancellationToken))
        {
            return Result<TeamDto>.Failure(TeamErrors.NameTaken);
        }

        return new TeamDto(team.Id, team.Name, []);
    }
}
