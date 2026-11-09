using FluentValidation;

namespace Cem.Crm.Application.Contacts;

public sealed class ListContactsValidator : AbstractValidator<ListContactsQuery>
{
    public ListContactsValidator()
    {
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
        RuleFor(q => q.Search).MaximumLength(100);
    }
}
