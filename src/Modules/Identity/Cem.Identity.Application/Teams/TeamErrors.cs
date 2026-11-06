using Cem.SharedKernel;

namespace Cem.Identity.Application.Teams;

internal static class TeamErrors
{
    public static readonly Error NotFound = Error.NotFound("teams.not_found", "Team not found.");

    public static readonly Error NameTaken = Error.Conflict("teams.name_taken", "Another team already has this name.");
}
