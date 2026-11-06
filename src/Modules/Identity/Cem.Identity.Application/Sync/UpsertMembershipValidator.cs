using FluentValidation;

namespace Cem.Identity.Application.Sync;

public sealed class UpsertMembershipValidator : AbstractValidator<UpsertMembershipCommand>
{
    public UpsertMembershipValidator()
    {
        RuleFor(c => c.ClerkOrgId).NotEmpty();
        RuleFor(c => c.ClerkUserId).NotEmpty();
        RuleFor(c => c.ClerkMembershipId).NotEmpty();
    }
}
