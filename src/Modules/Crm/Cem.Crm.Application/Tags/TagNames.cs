using FluentValidation;

namespace Cem.Crm.Application.Tags;

internal static class TagNames
{
    public const int MaxPerRecord = 20;
    public const int MaxLength = 50;

    /// <summary>Trimmed names without blanks or case-insensitive duplicates (the first spelling wins).</summary>
    public static List<string> Normalize(IEnumerable<string> names) =>
        names.Select(n => n.Trim()).Where(n => n.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

    /// <summary>
    /// The rules for the tags list of a create or update request (null is allowed). The list is reached through a
    /// delegate, so FluentValidation cannot infer the property name: it is set explicitly.
    /// </summary>
    public static void TagListRules<T>(this AbstractValidator<T> validator, Func<T, IReadOnlyList<string>?> tags)
    {
        validator.RuleFor(c => tags(c)).Must(t => Normalize(t!).Count <= MaxPerRecord)
            .When(c => tags(c) is not null)
            .WithMessage($"At most {MaxPerRecord} tags are allowed.")
            .OverridePropertyName("Tags");
        validator.RuleForEach(c => tags(c)).MaximumLength(MaxLength).OverridePropertyName("Tags");
    }
}
