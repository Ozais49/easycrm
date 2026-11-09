using Cem.Crm.Application.Abstractions;
using Cem.Crm.Domain;
using Cem.Crm.Application.Tags;
using Cem.Identity.Contracts;
using Cem.Settings.Contracts;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Contacts;

public sealed class CreateContactHandler(
    ICrmStore store, ICrmQueries queries, IMemberDirectory directory, ICustomFieldCatalog catalog,
    IPermissionService permissions, ICurrentUser user)
    : ICommandHandler<CreateContactCommand, Result<ContactDto>>
{
    public async ValueTask<Result<ContactDto>> Handle(CreateContactCommand command, CancellationToken cancellationToken)
    {
        var ownerUserId = command.OwnerUserId ?? user.UserId;
        if (ownerUserId is null)
        {
            return Result<ContactDto>.Failure(CrmErrors.Invalid("OwnerUserId", "An owner is required."));
        }

        if (!RecordAccess.For(Scope.Crm, PermissionAction.Create, permissions, user).Allows(ownerUserId.Value, command.TeamId))
        {
            return Result<ContactDto>.Failure(CrmErrors.OwnerOutsideCreateLevel);
        }

        var invalid = await OwnershipChecks.ValidateAsync(
            directory, ownerUserId.Value, command.TeamId, null, null, cancellationToken);
        if (invalid is not null)
        {
            return Result<ContactDto>.Failure(invalid);
        }

        if (command.CompanyId is { } companyId && !await store.CompanyExistsAsync(companyId, cancellationToken))
        {
            return Result<ContactDto>.Failure(CrmErrors.Invalid("CompanyId", "The company does not exist."));
        }

        var customFields = CustomFieldValues.Prepare(
            await catalog.GetActiveAsync(CustomFieldEntity.Contact, cancellationToken), command.CustomFields, "{}");
        if (customFields.IsFailure)
        {
            return Result<ContactDto>.Failure(customFields.Error);
        }

        var contact = Contact.Create(
            Text.Clean(command.FirstName)!,
            Text.Clean(command.LastName),
            Text.Clean(command.JobTitle),
            command.CompanyId,
            Text.Clean(command.Email),
            Text.Clean(command.Phone),
            Text.Clean(command.Mobile),
            AddressDto.ToDomain(command.Address),
            ownerUserId.Value,
            command.TeamId);

        contact.SetCustomFields(customFields.Value);
        store.Add(contact);
        if (command.Tags is not null)
        {
            var tags = await store.ResolveTagsAsync(TagNames.Normalize(command.Tags), cancellationToken);
            await store.SetTagsAsync(contact, tags, cancellationToken);
        }

        if (!await store.TrySaveAsync(cancellationToken))
        {
            return Result<ContactDto>.Failure(CrmErrors.TagsChangedConcurrently);
        }

        var created = await queries.GetContactAsync(contact.Id, RecordAccess.Unrestricted, cancellationToken);
        return created ?? throw new InvalidOperationException("A created contact could not be read back.");
    }
}
