namespace Cem.Identity.Endpoints.Teams;

public sealed record SetTeamMembersRequest(IReadOnlyList<Guid> MembershipIds);
