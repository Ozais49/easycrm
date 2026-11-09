using FluentValidation;

namespace Cem.Settings.Application.CustomFields;

public sealed class ListCustomFieldsValidator : AbstractValidator<ListCustomFieldsQuery>
{
    public ListCustomFieldsValidator() => RuleFor(q => q.EntityType).IsInEnum();
}
