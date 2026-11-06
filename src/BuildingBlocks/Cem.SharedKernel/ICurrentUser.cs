namespace Cem.SharedKernel;

/// <summary>The authenticated user of the current request; empty for webhooks and background work.</summary>
public interface ICurrentUser
{
    Guid? UserId { get; }

    string? ClerkUserId { get; }
}
