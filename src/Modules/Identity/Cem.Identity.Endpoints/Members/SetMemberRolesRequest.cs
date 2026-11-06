namespace Cem.Identity.Endpoints.Members;

public sealed record SetMemberRolesRequest(IReadOnlyList<Guid> RoleIds);
