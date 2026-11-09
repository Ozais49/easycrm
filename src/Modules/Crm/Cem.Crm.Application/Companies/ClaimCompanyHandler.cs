using Cem.Crm.Application.Abstractions;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Companies;

/// <summary>
/// Anyone who can see an unassigned company (its pool, see <see cref="RecordAccess"/>) and has Crm:Edit at some level
/// (checked by the endpoint) can take it up, even at "own" level.
/// </summary>
public sealed class ClaimCompanyHandler(
    ICrmStore store, ICrmQueries queries, IPermissionService permissions, ICurrentUser user)
    : ICommandHandler<ClaimCompanyCommand, Result<CompanyDto>>
{
    public async ValueTask<Result<CompanyDto>> Handle(ClaimCompanyCommand command, CancellationToken cancellationToken)
    {
        var company = await store.FindCompanyAsync(command.Id, cancellationToken);
        if (company is null || !RecordAccess.For(Scope.Crm, PermissionAction.Read, permissions, user).Allows(company))
        {
            return Result<CompanyDto>.Failure(CrmErrors.CompanyNotFound);
        }

        if (company.OwnerUserId is not null || user.UserId is not { } me)
        {
            return Result<CompanyDto>.Failure(CrmErrors.AlreadyAssigned);
        }

        company.Assign(me, company.TeamId);
        await store.SaveChangesAsync(cancellationToken);

        var claimed = await queries.GetCompanyAsync(company.Id, RecordAccess.Unrestricted, cancellationToken);
        return claimed ?? throw new InvalidOperationException("A claimed company could not be read back.");
    }
}
