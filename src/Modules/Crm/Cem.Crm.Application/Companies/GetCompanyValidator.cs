using FluentValidation;

namespace Cem.Crm.Application.Companies;

public sealed class GetCompanyValidator : AbstractValidator<GetCompanyQuery>
{
    public GetCompanyValidator() => RuleFor(q => q.Id).NotEmpty();
}
