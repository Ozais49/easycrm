using FluentValidation;

namespace Cem.Identity.Application.Sync;

public sealed class RemoveMembershipValidator : AbstractValidator<RemoveMembershipCommand>
{
    public RemoveMembershipValidator()
    {
        RuleFor(c => c.ClerkOrgId).NotEmpty();
        RuleFor(c => c.ClerkUserId).NotEmpty();
    }
}
