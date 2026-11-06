namespace Cem.Identity.Application.Teams;

public sealed record TeamDto(Guid Id, string Name, IReadOnlyList<Guid> MembershipIds);
