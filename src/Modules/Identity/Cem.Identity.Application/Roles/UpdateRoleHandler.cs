using Cem.Identity.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Roles;

public sealed class UpdateRoleHandler(IIdentityStore store, IPermissionCacheInvalidator permissionCache)
    : ICommandHandler<UpdateRoleCommand, Result<RoleDto>>
{
    public async ValueTask<Result<RoleDto>> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await store.FindRoleAsync(command.RoleId, cancellationToken);
        if (role is null)
        {
            return Result<RoleDto>.Failure(RoleErrors.NotFound);
        }

        var name = command.Name.Trim();
        if (await store.RoleNameTakenAsync(name, role.Id, cancellationToken))
        {
            return Result<RoleDto>.Failure(RoleErrors.NameTaken);
        }

        var updated = role.Update(name, command.Permissions);
        if (updated.IsFailure)
        {
            return Result<RoleDto>.Failure(updated.Error);
        }

        // A concurrent request may have taken the name between the check and the save.
        if (!await store.TrySaveNewAsync(cancellationToken))
        {
            return Result<RoleDto>.Failure(RoleErrors.NameTaken);
        }

        permissionCache.InvalidateTenant();

        return new RoleDto(role.Id, role.Name, role.IsSystem, role.Permissions);
    }
}
