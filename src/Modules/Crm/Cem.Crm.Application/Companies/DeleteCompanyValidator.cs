using FluentValidation;

namespace Cem.Crm.Application.Companies;

public sealed class DeleteCompanyValidator : AbstractValidator<DeleteCompanyCommand>
{
    public DeleteCompanyValidator() => RuleFor(c => c.Id).NotEmpty();
}
