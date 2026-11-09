using FluentValidation;

namespace Cem.Crm.Application.Companies;

public sealed class ClaimCompanyValidator : AbstractValidator<ClaimCompanyCommand>
{
    public ClaimCompanyValidator() => RuleFor(c => c.Id).NotEmpty();
}
