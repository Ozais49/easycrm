using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;
using Mediator;

namespace Cem.Identity.Application.Me;

public sealed class GetMeHandler(
    IIdentityQueries queries,
    IPermissionService permissions,
    ITenantContext tenantContext,
    ICurrentUser currentUser) : IQueryHandler<GetMeQuery, Result<MeDto>>
{
    public async ValueTask<Result<MeDto>> Handle(GetMeQuery query, CancellationToken cancellationToken)
    {
        var identity = await queries.GetMeAsync(currentUser.UserId!.Value, tenantContext.TenantId!.Value, cancellationToken);
        if (identity is null)
        {
            return Result<MeDto>.Failure(Error.NotFound("me.not_found", "Your profile could not be found."));
        }

        return new MeDto(identity.User, identity.Tenant, identity.Roles, permissions.TeamIds, permissions.Matrix);
    }
}
