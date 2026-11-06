using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sync;

public sealed class RemoveMembershipHandler(IIdentityStore store, IRequestContextSetter requestContext)
    : ICommandHandler<RemoveMembershipCommand, Result>
{
    public async ValueTask<Result> Handle(RemoveMembershipCommand command, CancellationToken cancellationToken)
    {
        var tenant = await store.FindTenantAsync(command.ClerkOrgId, cancellationToken);
        var user = await store.FindUserAsync(command.ClerkUserId, cancellationToken);
        if (tenant is null || user is null)
        {
            return Result.Success();
        }

        requestContext.SetTenant(tenant.Id);

        var membership = await store.FindMembershipAsync(user.Id, cancellationToken);
        if (membership is null)
        {
            return Result.Success();
        }

        membership.Remove();
        await store.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
