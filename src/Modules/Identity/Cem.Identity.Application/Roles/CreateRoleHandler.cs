using Cem.Identity.Application.Abstractions;
using Cem.Identity.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Roles;

public sealed class CreateRoleHandler(IIdentityStore store) : ICommandHandler<CreateRoleCommand, Result<RoleDto>>
{
    public async ValueTask<Result<RoleDto>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var name = command.Name.Trim();
        if (await store.RoleNameTakenAsync(name, exceptRoleId: null, cancellationToken))
        {
            return Result<RoleDto>.Failure(RoleErrors.NameTaken);
        }

        var role = Role.Create(name, command.Permissions);
        store.Add(role);
        // A concurrent request may have taken the name between the check and the save.
        if (!await store.TrySaveNewAsync(cancellationToken))
        {
            return Result<RoleDto>.Failure(RoleErrors.NameTaken);
        }

        return new RoleDto(role.Id, role.Name, role.IsSystem, role.Permissions);
    }
}
