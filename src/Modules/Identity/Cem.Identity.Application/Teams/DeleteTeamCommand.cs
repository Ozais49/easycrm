using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Teams;

public sealed record DeleteTeamCommand(Guid TeamId) : ICommand<Result>;
