using FluentValidation;

namespace Cem.Crm.Application.Companies;

public sealed class UpdateCompanyValidator : CompanyInputValidator<UpdateCompanyCommand>
{
    public UpdateCompanyValidator() => RuleFor(c => c.Id).NotEmpty();
}
