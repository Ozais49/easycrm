using Cem.Settings.Domain;
using FluentValidation;

namespace Cem.Settings.Application.CustomFields;

public sealed class CreateCustomFieldValidator : AbstractValidator<CreateCustomFieldCommand>
{
    public CreateCustomFieldValidator()
    {
        RuleFor(c => c.EntityType).IsInEnum();
        RuleFor(c => c.Type).IsInEnum();
        RuleFor(c => c.Label).NotEmpty().MaximumLength(80);
        RuleFor(c => c.Key).Matches("^[a-z][a-z0-9_]{0,39}$").When(c => !string.IsNullOrWhiteSpace(c.Key))
            .WithMessage("Key must start with a lowercase letter and use only a-z, 0-9 and _ (max 40).");
        RuleFor(c => c.Options).Must(o => o is { Count: >= 1 and <= 50 })
            .When(c => c.Type == CustomFieldType.Select)
            .WithMessage("A select field needs 1 to 50 options.");
        RuleFor(c => c.Options).Must(o => o is null or { Count: 0 })
            .When(c => c.Type != CustomFieldType.Select)
            .WithMessage("Only a select field has options.");
        this.OptionItemRules(c => c.Options);
    }
}
