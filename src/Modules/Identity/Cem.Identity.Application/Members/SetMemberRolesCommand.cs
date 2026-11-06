using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Members;

/// <summary>Replaces the roles of the member with exactly <paramref name="RoleIds"/>.</summary>
public sealed record SetMemberRolesCommand(Guid MembershipId, IReadOnlyList<Guid> RoleIds) : ICommand<Result>;
