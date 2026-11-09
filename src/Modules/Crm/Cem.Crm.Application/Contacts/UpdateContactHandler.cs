using Cem.Crm.Application.Abstractions;
using Cem.Crm.Application.Tags;
using Cem.Identity.Contracts;
using Cem.Settings.Contracts;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Contacts;

public sealed class UpdateContactHandler(
    ICrmStore store, ICrmQueries queries, IMemberDirectory directory, ICustomFieldCatalog catalog,
    IPermissionService permissions, ICurrentUser user)
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

        string? customFieldsJson = null;
        if (command.CustomFields is not null)
        {
            var customFields = CustomFieldValues.Prepare(
                await catalog.GetActiveAsync(CustomFieldEntity.Contact, cancellationToken),
                command.CustomFields,
                contact.CustomFields);
            if (customFields.IsFailure)
            {
                return Result<ContactDto>.Failure(customFields.Error);
            }

            customFieldsJson = customFields.Value;
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
        if (customFieldsJson is not null)
        {
            contact.SetCustomFields(customFieldsJson);
        }

        if (command.Tags is not null)
        {
            var tags = await store.ResolveTagsAsync(TagNames.Normalize(command.Tags), cancellationToken);
            await store.SetTagsAsync(contact, tags, cancellationToken);
        }

        if (!await store.TrySaveAsync(cancellationToken))
        {
            return Result<ContactDto>.Failure(CrmErrors.TagsChangedConcurrently);
        }

        var updated = await queries.GetContactAsync(contact.Id, RecordAccess.Unrestricted, cancellationToken);
        return updated ?? throw new InvalidOperationException("An updated contact could not be read back.");
    }
}
