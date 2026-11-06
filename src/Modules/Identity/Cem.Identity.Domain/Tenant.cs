using Cem.SharedKernel;

namespace Cem.Identity.Domain;

/// <summary>A client organization; mirrors one Clerk organization. Global table (no tenant filter, no RLS).</summary>
public sealed class Tenant : AuditableEntity, ISoftDeletable
{
    private Tenant()
    {
    }

    public string ClerkOrgId { get; private set; } = "";
    public string Name { get; private set; } = "";
    public string Slug { get; private set; } = "";
    public DateTimeOffset? DeletedAt { get; private set; }

    public static Tenant Create(string clerkOrgId, string name, string slug) =>
        new() { ClerkOrgId = clerkOrgId, Name = name, Slug = slug };

    public void Update(string name, string slug)
    {
        Name = name;
        Slug = slug;
    }

    public void MarkDeleted(DateTimeOffset now) => DeletedAt ??= now;
}
