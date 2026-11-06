using FluentValidation;

namespace Cem.Identity.Application.Teams;

public sealed class CreateTeamValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamValidator() => RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
}
