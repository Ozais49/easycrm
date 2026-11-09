using FluentValidation;

namespace Cem.Settings.Application.CustomFields;

internal static class CustomFieldOptionRules
{
    /// <summary>Each option: not blank, at most 80 characters; all unique ignoring case. Count rules depend on the type.</summary>
    public static void OptionItemRules<T>(this AbstractValidator<T> validator, Func<T, IReadOnlyList<string>?> options)
    {
        validator.RuleForEach(c => options(c)).OverridePropertyName("Options").NotEmpty().MaximumLength(80);
        validator.RuleFor(c => options(c)).OverridePropertyName("Options")
            .Must(o => o!.Select(v => v.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() == o!.Count)
            .When(c => options(c) is not null)
            .WithMessage("Options must be unique.");
    }
}
