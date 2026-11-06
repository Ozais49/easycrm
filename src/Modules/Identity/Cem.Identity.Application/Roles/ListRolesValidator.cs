using FluentValidation;

namespace Cem.Identity.Application.Roles;

public sealed class ListRolesValidator : AbstractValidator<ListRolesQuery>
{
    public ListRolesValidator()
    {
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
    }
}
