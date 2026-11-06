using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sync;

public sealed class UpsertMembershipHandler(IdentityProvisioner provisioner, IIdentityStore store, TimeProvider timeProvider)
    : ICommandHandler<UpsertMembershipCommand, Result>
{
    public async ValueTask<Result> Handle(UpsertMembershipCommand command, CancellationToken cancellationToken)
    {
        var tenant = await provisioner.EnsureTenantAsync(command.ClerkOrgId, slug: null, cancellationToken);
        var user = await provisioner.EnsureUserAsync(command.ClerkUserId, cancellationToken);
        if (tenant is null || user is null)
        {
            return Result.Success(); // soft-deleted organization or user; nothing to link
        }

        var membership = await provisioner.EnsureMembershipAsync(tenant, user, command.IsOrgAdmin, cancellationToken);
        membership.Activate(timeProvider.GetUtcNow());
        membership.LinkClerkMembership(command.ClerkMembershipId);
        await store.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
