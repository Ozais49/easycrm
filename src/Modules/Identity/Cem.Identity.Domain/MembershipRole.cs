using Cem.SharedKernel;

namespace Cem.Identity.Domain;

/// <summary>Assigns a <see cref="Role"/> to a <see cref="TenantMembership"/>.</summary>
public sealed class MembershipRole : Entity, ITenantEntity
{
    private MembershipRole()
    {
    }

    public Guid TenantId { get; private set; }
    public Guid MembershipId { get; private set; }
    public Guid RoleId { get; private set; }

    public static MembershipRole Create(Guid membershipId, Guid roleId) =>
        new() { MembershipId = membershipId, RoleId = roleId };
}
