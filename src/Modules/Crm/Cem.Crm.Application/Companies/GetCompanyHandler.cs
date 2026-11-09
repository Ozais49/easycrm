using Cem.Crm.Application.Abstractions;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Companies;

public sealed class GetCompanyHandler(ICrmQueries queries, IPermissionService permissions, ICurrentUser user)
    : IQueryHandler<GetCompanyQuery, Result<CompanyDto>>
{
    public async ValueTask<Result<CompanyDto>> Handle(GetCompanyQuery query, CancellationToken cancellationToken)
    {
        var company = await queries.GetCompanyAsync(
            query.Id, RecordAccess.For(Scope.Crm, PermissionAction.Read, permissions, user), cancellationToken);

        // Outside the read level is reported as missing: the caller must not learn the record exists.
        return company is null ? Result<CompanyDto>.Failure(CrmErrors.CompanyNotFound) : company;
    }
}
