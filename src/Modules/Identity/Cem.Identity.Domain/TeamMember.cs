using Cem.SharedKernel;

namespace Cem.Identity.Domain;

/// <summary>Puts a <see cref="TenantMembership"/> into a <see cref="Team"/>.</summary>
public sealed class TeamMember : Entity, ITenantEntity
{
    private TeamMember()
    {
    }

    public Guid TenantId { get; private set; }
    public Guid TeamId { get; private set; }
    public Guid MembershipId { get; private set; }

    public static TeamMember Create(Guid teamId, Guid membershipId) =>
        new() { TeamId = teamId, MembershipId = membershipId };
}
