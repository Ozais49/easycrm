using Cem.Crm.Application.Abstractions;
using Cem.Crm.Domain;
using Cem.Crm.Application.Tags;
using Cem.Identity.Contracts;
using Cem.Settings.Contracts;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Companies;

public sealed class CreateCompanyHandler(
    ICrmStore store, ICrmQueries queries, IMemberDirectory directory, ICustomFieldCatalog catalog,
    IPermissionService permissions, ICurrentUser user)
    : ICommandHandler<CreateCompanyCommand, Result<CompanyDto>>
{
    public async ValueTask<Result<CompanyDto>> Handle(CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        var ownerUserId = command.OwnerUserId ?? user.UserId;
        if (ownerUserId is null)
        {
            return Result<CompanyDto>.Failure(CrmErrors.Invalid("OwnerUserId", "An owner is required."));
        }

        if (!RecordAccess.For(Scope.Crm, PermissionAction.Create, permissions, user).Allows(ownerUserId.Value, command.TeamId))
        {
            return Result<CompanyDto>.Failure(CrmErrors.OwnerOutsideCreateLevel);
        }

        var invalid = await OwnershipChecks.ValidateAsync(
            directory, ownerUserId.Value, command.TeamId, null, null, cancellationToken);
        if (invalid is not null)
        {
            return Result<CompanyDto>.Failure(invalid);
        }

        var customFields = CustomFieldValues.Prepare(
            await catalog.GetActiveAsync(CustomFieldEntity.Company, cancellationToken), command.CustomFields, "{}");
        if (customFields.IsFailure)
        {
            return Result<CompanyDto>.Failure(customFields.Error);
        }

        var company = Company.Create(
            Text.Clean(command.Name)!,
            Text.Clean(command.Industry),
            Text.Clean(command.Website),
            Text.Clean(command.Phone),
            Text.Clean(command.Email),
            Text.Clean(command.PanNumber),
            AddressDto.ToDomain(command.Address),
            ownerUserId.Value,
            command.TeamId);

        company.SetCustomFields(customFields.Value);
        store.Add(company);
        if (command.Tags is not null)
        {
            var tags = await store.ResolveTagsAsync(TagNames.Normalize(command.Tags), cancellationToken);
            await store.SetTagsAsync(company, tags, cancellationToken);
        }

        if (!await store.TrySaveAsync(cancellationToken))
        {
            return Result<CompanyDto>.Failure(CrmErrors.TagsChangedConcurrently);
        }

        var created = await queries.GetCompanyAsync(company.Id, RecordAccess.Unrestricted, cancellationToken);
        return created ?? throw new InvalidOperationException("A created company could not be read back.");
    }
}
