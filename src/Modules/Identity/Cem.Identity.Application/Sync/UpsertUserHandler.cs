using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sync;

public sealed class UpsertUserHandler(IdentityProvisioner provisioner, IIdentityStore store)
    : ICommandHandler<UpsertUserCommand, Result>
{
    public async ValueTask<Result> Handle(UpsertUserCommand command, CancellationToken cancellationToken)
    {
        var user = await provisioner.EnsureUserAsync(command.ClerkUserId, cancellationToken);
        if (user is null)
        {
            return Result.Success(); // soft-deleted; nothing to update
        }

        user.UpdateProfile(command.Email, command.FirstName, command.LastName, command.ImageUrl);
        await store.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
