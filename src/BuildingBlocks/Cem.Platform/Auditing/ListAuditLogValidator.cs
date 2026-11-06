using FluentValidation;

namespace Cem.Platform.Auditing;

public sealed class ListAuditLogValidator : AbstractValidator<ListAuditLogQuery>
{
    public ListAuditLogValidator()
    {
        RuleFor(q => q.EntityType).MaximumLength(100);
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
    }
}
