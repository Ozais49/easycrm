using FluentValidation;

namespace Cem.Identity.Application.Roles;

public sealed class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleValidator() => RuleFor(c => c.RoleId).NotEmpty();
}
