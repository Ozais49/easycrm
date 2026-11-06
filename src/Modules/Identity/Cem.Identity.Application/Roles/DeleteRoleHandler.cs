using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Roles;

public sealed class DeleteRoleHandler(IIdentityStore store) : ICommandHandler<DeleteRoleCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await store.FindRoleAsync(command.RoleId, cancellationToken);
        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        var deletable = role.EnsureCanBeDeleted();
        if (deletable.IsFailure)
        {
            return deletable;
        }

        if (await store.RoleIsAssignedAsync(role.Id, cancellationToken))
        {
            return Result.Failure(Error.Conflict("roles.in_use", "This role is still assigned to members."));
        }

        store.Remove(role);
        await store.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
