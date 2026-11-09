using FluentValidation;

namespace Cem.Settings.Application.CustomFields;

public sealed class ReorderCustomFieldsValidator : AbstractValidator<ReorderCustomFieldsCommand>
{
    public ReorderCustomFieldsValidator()
    {
        RuleFor(c => c.EntityType).IsInEnum();
        RuleFor(c => c.Ids).NotNull()
            .Must(ids => ids.Count <= CustomFieldErrors.MaxPerEntity && ids.Distinct().Count() == ids.Count)
            .WithMessage("Ids must be distinct.");
    }
}
