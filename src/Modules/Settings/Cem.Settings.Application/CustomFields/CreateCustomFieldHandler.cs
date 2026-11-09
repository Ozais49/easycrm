using System.Text;
using System.Text.RegularExpressions;
using Cem.Settings.Application.Abstractions;
using Cem.Settings.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.CustomFields;

public sealed partial class CreateCustomFieldHandler(ICustomFieldStore store)
    : ICommandHandler<CreateCustomFieldCommand, Result<CustomFieldDto>>
{
    public async ValueTask<Result<CustomFieldDto>> Handle(CreateCustomFieldCommand command, CancellationToken cancellationToken)
    {
        var active = await store.ListActiveAsync(command.EntityType, cancellationToken);
        if (active.Count >= CustomFieldErrors.MaxPerEntity)
        {
            return Result<CustomFieldDto>.Failure(CustomFieldErrors.Invalid(
                "Label", $"At most {CustomFieldErrors.MaxPerEntity} custom fields are allowed."));
        }

        var taken = active.Select(d => d.Key).ToHashSet();
        string key;
        if (string.IsNullOrWhiteSpace(command.Key))
        {
            key = UniqueKey(Slug(command.Label), taken);
        }
        else if (taken.Contains(command.Key))
        {
            return Result<CustomFieldDto>.Failure(CustomFieldErrors.Invalid("Key", "This key is already used."));
        }
        else
        {
            key = command.Key;
        }

        var options = command.Type == CustomFieldType.Select
            ? command.Options!.Select(o => o.Trim()).ToArray()
            : [];

        var definition = CustomFieldDefinition.Create(
            command.EntityType, key, command.Label.Trim(), command.Type, command.Required, options,
            command.ShowInList, active.Count == 0 ? 0 : active.Max(d => d.SortOrder) + 1);

        store.Add(definition);
        if (!await store.TrySaveNewAsync(cancellationToken))
        {
            return Result<CustomFieldDto>.Failure(CustomFieldErrors.Invalid("Key", "This key is already used."));
        }

        return CustomFieldDto.From(definition);
    }

    /// <summary>Lowercase ascii slug of the label; falls back to <c>field</c> and never starts with a digit.</summary>
    private static string Slug(string label)
    {
        var slug = new StringBuilder();
        foreach (var c in label.Trim().ToLowerInvariant())
        {
            slug.Append(c is >= 'a' and <= 'z' or >= '0' and <= '9' ? c : '_');
        }

        var text = Underscores().Replace(slug.ToString(), "_").Trim('_');
        if (text.Length == 0 || !char.IsAsciiLetter(text[0]))
        {
            text = "f_" + text;
        }

        return text[..Math.Min(text.Length, 40)].TrimEnd('_');
    }

    private static string UniqueKey(string baseKey, HashSet<string> taken)
    {
        if (!taken.Contains(baseKey))
        {
            return baseKey;
        }

        for (var n = 2; ; n++)
        {
            var suffix = $"_{n}";
            var candidate = baseKey[..Math.Min(baseKey.Length, 40 - suffix.Length)] + suffix;
            if (!taken.Contains(candidate))
            {
                return candidate;
            }
        }
    }

    [GeneratedRegex("_+")]
    private static partial Regex Underscores();
}
