using FluentValidation;

namespace Cem.Crm.Application.Companies;

public sealed class ListCompaniesValidator : AbstractValidator<ListCompaniesQuery>
{
    public ListCompaniesValidator()
    {
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
        RuleFor(q => q.Search).MaximumLength(100);
    }
}
