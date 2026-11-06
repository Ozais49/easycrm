using Cem.Identity.Application.Abstractions;
using Cem.Identity.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Members;

public sealed class SetMemberRolesHandler(IIdentityStore store, IPermissionCacheInvalidator permissionCache)
    : ICommandHandler<SetMemberRolesCommand, Result>
{
    public async ValueTask<Result> Handle(SetMemberRolesCommand command, CancellationToken cancellationToken)
    {
        var membership = await store.FindMembershipByIdAsync(command.MembershipId, cancellationToken);
        if (membership is null)
        {
            return Result.Failure(Error.NotFound("members.not_found", "Member not found."));
        }

        var wanted = command.RoleIds.Distinct().ToList();
        if (await store.CountRolesAsync(wanted, cancellationToken) != wanted.Count)
        {
            return Result.Failure(Error.Validation(
                "roles.not_found",
                "One or more roles do not exist.",
                new Dictionary<string, string[]> { ["roleIds"] = ["One or more roles do not exist."] }));
        }

        var current = await store.ListMembershipRolesAsync(membership.Id, cancellationToken);

        if (membership.IsActive && await WouldRemoveLastAdminAsync(membership, current, wanted, cancellationToken))
        {
            return Result.Failure(Error.Conflict("roles.last_admin", "The organization needs at least one active Admin."));
        }

        foreach (var assignment in current.Where(a => !wanted.Contains(a.RoleId)))
        {
            store.Remove(assignment);
        }

        foreach (var roleId in wanted.Where(id => current.All(a => a.RoleId != id)))
        {
            store.Add(MembershipRole.Create(membership.Id, roleId));
        }

        await store.SaveChangesAsync(cancellationToken);
        permissionCache.InvalidateTenant();
        return Result.Success();
    }

    private async Task<bool> WouldRemoveLastAdminAsync(
        TenantMembership membership,
        List<MembershipRole> current,
        List<Guid> wanted,
        CancellationToken cancellationToken)
    {
        var admin = await store.FindAdminRoleAsync(cancellationToken);

        return admin is not null
            && current.Any(a => a.RoleId == admin.Id)
            && !wanted.Contains(admin.Id)
            && !await store.OtherActiveAdminExistsAsync(membership.Id, cancellationToken);
    }
}
