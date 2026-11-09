using Cem.Crm.Application.Abstractions;
using Cem.Crm.Application.Tags;
using Cem.Identity.Contracts;
using Cem.Settings.Contracts;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Companies;

public sealed class UpdateCompanyHandler(
    ICrmStore store, ICrmQueries queries, IMemberDirectory directory, ICustomFieldCatalog catalog,
    IPermissionService permissions, ICurrentUser user)
    : ICommandHandler<UpdateCompanyCommand, Result<CompanyDto>>
{
    public async ValueTask<Result<CompanyDto>> Handle(UpdateCompanyCommand command, CancellationToken cancellationToken)
    {
        var company = await store.FindCompanyAsync(command.Id, cancellationToken);
        if (company is null || !RecordAccess.For(Scope.Crm, PermissionAction.Read, permissions, user).Allows(company))
        {
            return Result<CompanyDto>.Failure(CrmErrors.CompanyNotFound);
        }

        if (!RecordAccess.For(Scope.Crm, PermissionAction.Edit, permissions, user).Allows(company))
        {
            return Result<CompanyDto>.Failure(CrmErrors.PermissionDenied);
        }

        var ownerUserId = command.OwnerUserId ?? company.OwnerUserId;
        var invalid = await OwnershipChecks.ValidateAsync(
            directory, ownerUserId, command.TeamId, company.OwnerUserId, company.TeamId, cancellationToken);
        if (invalid is not null)
        {
            return Result<CompanyDto>.Failure(invalid);
        }

        string? customFieldsJson = null;
        if (command.CustomFields is not null)
        {
            var customFields = CustomFieldValues.Prepare(
                await catalog.GetActiveAsync(CustomFieldEntity.Company, cancellationToken),
                command.CustomFields,
                company.CustomFields);
            if (customFields.IsFailure)
            {
                return Result<CompanyDto>.Failure(customFields.Error);
            }

            customFieldsJson = customFields.Value;
        }

        company.Update(
            Text.Clean(command.Name)!,
            Text.Clean(command.Industry),
            Text.Clean(command.Website),
            Text.Clean(command.Phone),
            Text.Clean(command.Email),
            Text.Clean(command.PanNumber),
            AddressDto.ToDomain(command.Address));
        company.Assign(ownerUserId, command.TeamId);
        if (customFieldsJson is not null)
        {
            company.SetCustomFields(customFieldsJson);
        }

        if (command.Tags is not null)
        {
            var tags = await store.ResolveTagsAsync(TagNames.Normalize(command.Tags), cancellationToken);
            await store.SetTagsAsync(company, tags, cancellationToken);
        }

        if (!await store.TrySaveAsync(cancellationToken))
        {
            return Result<CompanyDto>.Failure(CrmErrors.TagsChangedConcurrently);
        }

        var updated = await queries.GetCompanyAsync(company.Id, RecordAccess.Unrestricted, cancellationToken);
        return updated ?? throw new InvalidOperationException("An updated company could not be read back.");
    }
}
