using FluentValidation;

namespace Cem.Identity.Application.Sync;

public sealed class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator() => RuleFor(c => c.ClerkUserId).NotEmpty();
}
