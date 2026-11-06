using FluentValidation;

namespace Cem.Identity.Application.Sync;

public sealed class UpsertUserValidator : AbstractValidator<UpsertUserCommand>
{
    public UpsertUserValidator() => RuleFor(c => c.ClerkUserId).NotEmpty();
}
