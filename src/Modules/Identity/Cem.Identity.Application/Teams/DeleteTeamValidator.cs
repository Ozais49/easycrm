using FluentValidation;

namespace Cem.Identity.Application.Teams;

public sealed class DeleteTeamValidator : AbstractValidator<DeleteTeamCommand>
{
    public DeleteTeamValidator() => RuleFor(c => c.TeamId).NotEmpty();
}
