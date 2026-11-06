using FluentValidation;

namespace Cem.Identity.Application.Members;

public sealed class SetMemberRolesValidator : AbstractValidator<SetMemberRolesCommand>
{
    public SetMemberRolesValidator()
    {
        RuleFor(c => c.MembershipId).NotEmpty();
        RuleFor(c => c.RoleIds).NotEmpty().WithMessage("At least one role is required.");
        RuleForEach(c => c.RoleIds).NotEmpty();
    }
}
