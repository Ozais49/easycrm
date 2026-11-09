using Cem.Crm.Application.Abstractions;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Contacts;

/// <summary>
/// Anyone who can see an unassigned contact (its pool, see <see cref="RecordAccess"/>) and has Crm:Edit at some level
/// (checked by the endpoint) can take it up, even at "own" level.
/// </summary>
public sealed class ClaimContactHandler(
    ICrmStore store, ICrmQueries queries, IPermissionService permissions, ICurrentUser user)
    : ICommandHandler<ClaimContactCommand, Result<ContactDto>>
{
    public async ValueTask<Result<ContactDto>> Handle(ClaimContactCommand command, CancellationToken cancellationToken)
    {
        var contact = await store.FindContactAsync(command.Id, cancellationToken);
        if (contact is null || !RecordAccess.For(Scope.Crm, PermissionAction.Read, permissions, user).Allows(contact))
        {
            return Result<ContactDto>.Failure(CrmErrors.ContactNotFound);
        }

        if (contact.OwnerUserId is not null || user.UserId is not { } me)
        {
            return Result<ContactDto>.Failure(CrmErrors.AlreadyAssigned);
        }

        contact.Assign(me, contact.TeamId);
        await store.SaveChangesAsync(cancellationToken);

        var claimed = await queries.GetContactAsync(contact.Id, RecordAccess.Unrestricted, cancellationToken);
        return claimed ?? throw new InvalidOperationException("A claimed contact could not be read back.");
    }
}
