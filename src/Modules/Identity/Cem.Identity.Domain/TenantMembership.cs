using Cem.SharedKernel;

namespace Cem.Identity.Domain;

/// <summary>A user's membership of a tenant. <see cref="TenantId"/> is stamped on save from the tenant context.</summary>
public sealed class TenantMembership : AuditableEntity, ITenantEntity
{
    private TenantMembership()
    {
    }

    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }

    /// <summary>Null for memberships provisioned from a session token before the Clerk webhook arrived.</summary>
    public string? ClerkMembershipId { get; private set; }

    public MembershipStatus Status { get; private set; }
    public DateTimeOffset JoinedAt { get; private set; }

    public bool IsActive => Status == MembershipStatus.Active;

    public static TenantMembership Create(Guid userId, DateTimeOffset now) =>
        new() { UserId = userId, Status = MembershipStatus.Active, JoinedAt = now };

    public void LinkClerkMembership(string clerkMembershipId) => ClerkMembershipId = clerkMembershipId;

    /// <summary>Re-joining after removal starts a new membership period.</summary>
    public void Activate(DateTimeOffset now)
    {
        if (IsActive)
        {
            return;
        }

        Status = MembershipStatus.Active;
        JoinedAt = now;
    }

    public void Remove() => Status = MembershipStatus.Removed;
}
