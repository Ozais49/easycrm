using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sync;

public sealed class DeleteTenantHandler(IIdentityStore store, TimeProvider timeProvider)
    : ICommandHandler<DeleteTenantCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteTenantCommand command, CancellationToken cancellationToken)
    {
        var tenant = await store.FindTenantAsync(command.ClerkOrgId, cancellationToken);
        if (tenant is null)
        {
            return Result.Success();
        }

        tenant.MarkDeleted(timeProvider.GetUtcNow());
        await store.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
