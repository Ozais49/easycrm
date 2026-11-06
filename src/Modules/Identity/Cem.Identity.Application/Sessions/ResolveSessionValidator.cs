using FluentValidation;

namespace Cem.Identity.Application.Sessions;

public sealed class ResolveSessionValidator : AbstractValidator<ResolveSessionCommand>
{
    public ResolveSessionValidator()
    {
        RuleFor(c => c.ClerkUserId).NotEmpty();
        RuleFor(c => c.ClerkOrgId).NotEmpty();
    }
}
