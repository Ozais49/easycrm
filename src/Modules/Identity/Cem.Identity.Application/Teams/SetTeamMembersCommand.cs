using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Teams;

/// <summary>Replaces the members of the team with exactly <paramref name="MembershipIds"/>; an empty list empties the team.</summary>
public sealed record SetTeamMembersCommand(Guid TeamId, IReadOnlyList<Guid> MembershipIds) : ICommand<Result>;
