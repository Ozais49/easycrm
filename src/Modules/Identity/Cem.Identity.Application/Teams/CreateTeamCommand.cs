using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Teams;

public sealed record CreateTeamCommand(string Name) : ICommand<Result<TeamDto>>;
