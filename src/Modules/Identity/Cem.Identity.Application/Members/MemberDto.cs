using Cem.Identity.Domain;

namespace Cem.Identity.Application.Members;

public sealed record MemberDto(
    Guid MembershipId,
    Guid UserId,
    string? Email,
    string? FirstName,
    string? LastName,
    MembershipStatus Status,
    IReadOnlyList<Guid> RoleIds);
