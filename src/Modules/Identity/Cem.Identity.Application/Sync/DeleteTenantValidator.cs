using FluentValidation;

namespace Cem.Identity.Application.Sync;

public sealed class DeleteTenantValidator : AbstractValidator<DeleteTenantCommand>
{
    public DeleteTenantValidator() => RuleFor(c => c.ClerkOrgId).NotEmpty();
}
