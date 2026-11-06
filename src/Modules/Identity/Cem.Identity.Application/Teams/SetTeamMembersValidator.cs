using FluentValidation;

namespace Cem.Identity.Application.Teams;

public sealed class SetTeamMembersValidator : AbstractValidator<SetTeamMembersCommand>
{
    public SetTeamMembersValidator()
    {
        RuleFor(c => c.TeamId).NotEmpty();
        RuleFor(c => c.MembershipIds).NotNull();
        RuleForEach(c => c.MembershipIds).NotEmpty();
    }
}
