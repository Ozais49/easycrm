using Cem.Identity.Contracts;
using Cem.SharedKernel;

namespace Cem.Crm.Application;

internal static class OwnershipChecks
{
    /// <summary>
    /// Owner must be an active member and the team must exist, both in the current tenant. Values equal to the
    /// current ones are not checked again, so editing a record whose owner has since left still works.
    /// </summary>
    public static async Task<Error?> ValidateAsync(
        IMemberDirectory directory,
        Guid ownerUserId,
        Guid? teamId,
        Guid? currentOwnerUserId,
        Guid? currentTeamId,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        if (ownerUserId != currentOwnerUserId && !await directory.IsActiveMemberAsync(ownerUserId, cancellationToken))
        {
            errors["OwnerUserId"] = ["The owner must be an active member of this organization."];
        }

        if (teamId is { } team && team != currentTeamId && !await directory.TeamExistsAsync(team, cancellationToken))
        {
            errors["TeamId"] = ["The team does not exist."];
        }

        return errors.Count == 0
            ? null
            : Error.Validation("validation.failed", "One or more validation errors occurred.", errors);
    }
}
