using FluentValidation;

namespace Cem.Settings.Application.CustomFields;

internal static class CustomFieldOptionRules
{
    /// <summary>Each option: not blank, at most 80 characters; all unique ignoring case. Count rules depend on the type.</summary>
    public static void OptionItemRules<T>(this AbstractValidator<T> validator, Func<T, IReadOnlyList<string>?> options)
    {
        // The list is reached through a delegate, so FluentValidation cannot infer the property name: it is set explicitly.
        validator.RuleForEach(c => options(c)).NotEmpty().MaximumLength(80).OverridePropertyName("Options");
        validator.RuleFor(c => options(c))
            .Must(o => o!.Select(v => v.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() == o!.Count)
            .When(c => options(c) is not null)
            .WithMessage("Options must be unique.")
            .OverridePropertyName("Options");
    }
}
