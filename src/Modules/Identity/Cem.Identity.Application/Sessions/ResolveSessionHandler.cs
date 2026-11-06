using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sessions;

public sealed class ResolveSessionHandler(IdentityProvisioner provisioner)
    : ICommandHandler<ResolveSessionCommand, Result<SessionInfo>>
{
    public async ValueTask<Result<SessionInfo>> Handle(ResolveSessionCommand command, CancellationToken cancellationToken)
    {
        var tenant = await provisioner.EnsureTenantAsync(command.ClerkOrgId, command.OrgSlug, cancellationToken);
        if (tenant is null)
        {
            return Result<SessionInfo>.Failure(Error.Forbidden("tenant.not_found", "This organization is no longer active."));
        }

        var user = await provisioner.EnsureUserAsync(command.ClerkUserId, cancellationToken);
        if (user is null)
        {
            return Result<SessionInfo>.Failure(Error.Forbidden("user.not_found", "This user is no longer active."));
        }

        var membership = await provisioner.EnsureMembershipAsync(tenant, user, command.IsOrgAdmin, cancellationToken);
        if (!membership.IsActive)
        {
            return Result<SessionInfo>.Failure(Error.Forbidden("tenant.membership_inactive", "Your membership of this organization is not active."));
        }

        return new SessionInfo(tenant.Id, user.Id);
    }
}
