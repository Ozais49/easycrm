using FluentValidation;

namespace Cem.Identity.Application.Roles;

public sealed class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Permissions).NotNull();
    }
}
