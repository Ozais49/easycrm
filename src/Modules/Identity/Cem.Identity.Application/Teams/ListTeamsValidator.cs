using FluentValidation;

namespace Cem.Identity.Application.Teams;

public sealed class ListTeamsValidator : AbstractValidator<ListTeamsQuery>
{
    public ListTeamsValidator()
    {
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
    }
}
