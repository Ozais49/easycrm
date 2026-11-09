namespace Cem.Identity.Contracts;

/// <summary>Lets other modules validate members and teams of the current tenant without touching Identity internals.</summary>
public interface IMemberDirectory
{
    /// <summary>True when the user has an active membership in the current tenant.</summary>
    Task<bool> IsActiveMemberAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>True when the team exists (and is not deleted) in the current tenant.</summary>
    Task<bool> TeamExistsAsync(Guid teamId, CancellationToken cancellationToken);
}
