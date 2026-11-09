using FluentValidation;

namespace Cem.Settings.Application.CustomFields;

/// <summary>The option count depends on the stored type (select or not), so the handler checks that part.</summary>
public sealed class UpdateCustomFieldValidator : AbstractValidator<UpdateCustomFieldCommand>
{
    public UpdateCustomFieldValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Label).NotEmpty().MaximumLength(80);
        RuleFor(c => c.Options).Must(o => o is null or { Count: <= 50 })
            .WithMessage("A select field needs 1 to 50 options.");
        this.OptionItemRules(c => c.Options);
    }
}
