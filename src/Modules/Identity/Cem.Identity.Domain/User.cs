using Cem.SharedKernel;

namespace Cem.Identity.Domain;

/// <summary>A person; mirrors one Clerk user. Global table (no tenant filter, no RLS): one user can join many tenants.</summary>
public sealed class User : AuditableEntity, ISoftDeletable
{
    private User()
    {
    }

    public string ClerkUserId { get; private set; } = "";

    /// <summary>Unknown until the Clerk webhook arrives: session tokens do not carry it.</summary>
    public string? Email { get; private set; }

    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? ImageUrl { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    /// <summary>Creates a user known only by its Clerk id; profile details arrive via <see cref="UpdateProfile"/>.</summary>
    public static User Create(string clerkUserId) => new() { ClerkUserId = clerkUserId };

    public void UpdateProfile(string? email, string? firstName, string? lastName, string? imageUrl)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        ImageUrl = imageUrl;
    }

    public void MarkDeleted(DateTimeOffset now) => DeletedAt ??= now;
}
