using Cem.Crm.Application.Abstractions;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Contacts;

public sealed class DeleteContactHandler(
    ICrmStore store, IPermissionService permissions, ICurrentUser user, TimeProvider timeProvider)
    : ICommandHandler<DeleteContactCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteContactCommand command, CancellationToken cancellationToken)
    {
        var contact = await store.FindContactAsync(command.Id, cancellationToken);
        if (contact is null || !RecordAccess.For(Scope.Crm, PermissionAction.Read, permissions, user).Allows(contact))
        {
            return Result.Failure(CrmErrors.ContactNotFound);
        }

        if (!RecordAccess.For(Scope.Crm, PermissionAction.Delete, permissions, user).Allows(contact))
        {
            return Result.Failure(CrmErrors.PermissionDenied);
        }

        contact.MarkDeleted(timeProvider.GetUtcNow());
        await store.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
