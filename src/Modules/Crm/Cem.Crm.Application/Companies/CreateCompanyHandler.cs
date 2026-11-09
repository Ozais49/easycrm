using Cem.Crm.Application.Abstractions;
using Cem.Crm.Domain;
using Cem.Identity.Contracts;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Companies;

public sealed class CreateCompanyHandler(
    ICrmStore store, ICrmQueries queries, IMemberDirectory directory, IPermissionService permissions, ICurrentUser user)
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

        store.Add(company);
        await store.SaveChangesAsync(cancellationToken);

        var created = await queries.GetCompanyAsync(company.Id, RecordAccess.Unrestricted, cancellationToken);
        return created ?? throw new InvalidOperationException("A created company could not be read back.");
    }
}
