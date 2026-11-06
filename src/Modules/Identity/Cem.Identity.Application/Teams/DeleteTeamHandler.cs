using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Teams;

public sealed class DeleteTeamHandler(IIdentityStore store, IPermissionCacheInvalidator permissionCache, TimeProvider timeProvider)
    : ICommandHandler<DeleteTeamCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteTeamCommand command, CancellationToken cancellationToken)
    {
        var team = await store.FindTeamAsync(command.TeamId, cancellationToken);
        if (team is null)
        {
            return Result.Failure(TeamErrors.NotFound);
        }

        team.MarkDeleted(timeProvider.GetUtcNow());
        await store.SaveChangesAsync(cancellationToken);
        permissionCache.InvalidateTenant();
        return Result.Success();
    }
}
