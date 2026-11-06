using FluentValidation;

namespace Cem.Identity.Application.Members;

public sealed class ListMembersValidator : AbstractValidator<ListMembersQuery>
{
    public ListMembersValidator()
    {
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
    }
}
