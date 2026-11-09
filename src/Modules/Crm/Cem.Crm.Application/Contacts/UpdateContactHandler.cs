using Cem.Crm.Application.Abstractions;
using Cem.Identity.Contracts;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Contacts;

public sealed class UpdateContactHandler(
    ICrmStore store, ICrmQueries queries, IMemberDirectory directory, IPermissionService permissions, ICurrentUser user)
    : ICommandHandler<UpdateContactCommand, Result<ContactDto>>
{
    public async ValueTask<Result<ContactDto>> Handle(UpdateContactCommand command, CancellationToken cancellationToken)
    {
        var contact = await store.FindContactAsync(command.Id, cancellationToken);
        if (contact is null || !RecordAccess.For(Scope.Crm, PermissionAction.Read, permissions, user).Allows(contact))
        {
            return Result<ContactDto>.Failure(CrmErrors.ContactNotFound);
        }

        if (!RecordAccess.For(Scope.Crm, PermissionAction.Edit, permissions, user).Allows(contact))
        {
            return Result<ContactDto>.Failure(CrmErrors.PermissionDenied);
        }

        var ownerUserId = command.OwnerUserId ?? contact.OwnerUserId;
        var invalid = await OwnershipChecks.ValidateAsync(
            directory, ownerUserId, command.TeamId, contact.OwnerUserId, contact.TeamId, cancellationToken);
        if (invalid is not null)
        {
            return Result<ContactDto>.Failure(invalid);
        }

        // A link that did not change is not checked again: the company may have been deleted since.
        if (command.CompanyId is { } companyId && companyId != contact.CompanyId
            && !await store.CompanyExistsAsync(companyId, cancellationToken))
        {
            return Result<ContactDto>.Failure(CrmErrors.Invalid("CompanyId", "The company does not exist."));
        }

        contact.Update(
            Text.Clean(command.FirstName)!,
            Text.Clean(command.LastName),
            Text.Clean(command.JobTitle),
            command.CompanyId);

        // Callers without ContactPii edit access send none of it; keep what is stored instead of clearing it.
        if (permissions.CanEditField(FieldGroup.ContactPii))
        {
            contact.SetContactDetails(
                Text.Clean(command.Email),
                Text.Clean(command.Phone),
                Text.Clean(command.Mobile),
                AddressDto.ToDomain(command.Address));
        }

        contact.Assign(ownerUserId, command.TeamId);
        await store.SaveChangesAsync(cancellationToken);

        var updated = await queries.GetContactAsync(contact.Id, RecordAccess.Unrestricted, cancellationToken);
        return updated ?? throw new InvalidOperationException("An updated contact could not be read back.");
    }
}
