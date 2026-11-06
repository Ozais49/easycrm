using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sync;

public sealed class UpsertTenantHandler(IdentityProvisioner provisioner, IIdentityStore store)
    : ICommandHandler<UpsertTenantCommand, Result>
{
    public async ValueTask<Result> Handle(UpsertTenantCommand command, CancellationToken cancellationToken)
    {
        var tenant = await provisioner.EnsureTenantAsync(command.ClerkOrgId, command.Slug, cancellationToken);
        if (tenant is null)
        {
            return Result.Success(); // soft-deleted; nothing to update
        }

        tenant.Update(command.Name, command.Slug);
        await store.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
