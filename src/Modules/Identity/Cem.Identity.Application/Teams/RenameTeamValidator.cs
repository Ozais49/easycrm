using FluentValidation;

namespace Cem.Identity.Application.Teams;

public sealed class RenameTeamValidator : AbstractValidator<RenameTeamCommand>
{
    public RenameTeamValidator()
    {
        RuleFor(c => c.TeamId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
    }
}
