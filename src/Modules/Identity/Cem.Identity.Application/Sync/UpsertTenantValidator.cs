using FluentValidation;

namespace Cem.Identity.Application.Sync;

public sealed class UpsertTenantValidator : AbstractValidator<UpsertTenantCommand>
{
    public UpsertTenantValidator()
    {
        RuleFor(c => c.ClerkOrgId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.Slug).NotEmpty();
    }
}
