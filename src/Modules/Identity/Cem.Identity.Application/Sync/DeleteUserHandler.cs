using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sync;

public sealed class DeleteUserHandler(IIdentityStore store, TimeProvider timeProvider)
    : ICommandHandler<DeleteUserCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await store.FindUserAsync(command.ClerkUserId, cancellationToken);
        if (user is null)
        {
            return Result.Success();
        }

        user.MarkDeleted(timeProvider.GetUtcNow());
        await store.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
