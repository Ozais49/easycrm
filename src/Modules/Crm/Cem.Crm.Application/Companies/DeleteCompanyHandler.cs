using Cem.Crm.Application.Abstractions;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Companies;

/// <summary>Soft delete. Its contacts keep their company id; the company simply no longer resolves.</summary>
public sealed class DeleteCompanyHandler(
    ICrmStore store, IPermissionService permissions, ICurrentUser user, TimeProvider timeProvider)
    : ICommandHandler<DeleteCompanyCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteCompanyCommand command, CancellationToken cancellationToken)
    {
        var company = await store.FindCompanyAsync(command.Id, cancellationToken);
        if (company is null || !RecordAccess.For(Scope.Crm, PermissionAction.Read, permissions, user).Allows(company))
        {
            return Result.Failure(CrmErrors.CompanyNotFound);
        }

        if (!RecordAccess.For(Scope.Crm, PermissionAction.Delete, permissions, user).Allows(company))
        {
            return Result.Failure(CrmErrors.PermissionDenied);
        }

        company.MarkDeleted(timeProvider.GetUtcNow());
        await store.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
