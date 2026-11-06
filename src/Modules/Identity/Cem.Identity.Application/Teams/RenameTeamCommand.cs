using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Teams;

public sealed record RenameTeamCommand(Guid TeamId, string Name) : ICommand<Result>;
