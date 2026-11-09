using Cem.Crm.Application.Abstractions;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Crm.Application.Contacts;

public sealed class GetContactHandler(ICrmQueries queries, IPermissionService permissions, ICurrentUser user)
    : IQueryHandler<GetContactQuery, Result<ContactDto>>
{
    public async ValueTask<Result<ContactDto>> Handle(GetContactQuery query, CancellationToken cancellationToken)
    {
        var contact = await queries.GetContactAsync(
            query.Id, RecordAccess.For(Scope.Crm, PermissionAction.Read, permissions, user), cancellationToken);

        // Outside the read level is reported as missing: the caller must not learn the record exists.
        return contact is null ? Result<ContactDto>.Failure(CrmErrors.ContactNotFound) : contact;
    }
}
