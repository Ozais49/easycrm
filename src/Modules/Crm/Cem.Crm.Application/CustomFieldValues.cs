using System.Globalization;
using System.Text.Json;
using Cem.Settings.Contracts;
using Cem.SharedKernel;

namespace Cem.Crm.Application;

/// <summary>Validates custom field input against the active definitions and reads stored values back.</summary>
public static class CustomFieldValues
{
    private const int MaxTextLength = 1000;
    private const string Prefix = "customFields.";

    /// <summary>
    /// The JSON to store: the input values of the active definitions, plus the stored values of definitions that
    /// no longer exist (kept untouched). Absent or null clears a value; required fields must have one.
    /// A required checkbox must be checked.
    /// </summary>
    internal static Result<string> Prepare(
        IReadOnlyList<CustomFieldDefinitionInfo> active,
        IReadOnlyDictionary<string, JsonElement>? input,
        string storedJson)
    {
        input ??= new Dictionary<string, JsonElement>();
        var errors = new Dictionary<string, string[]>();
        var byKey = active.ToDictionary(d => d.Key);

        foreach (var key in input.Keys.Where(k => !byKey.ContainsKey(k)))
        {
            errors[Prefix + key] = ["Unknown custom field."];
        }

        var values = new Dictionary<string, object>();
        using (var stored = JsonDocument.Parse(storedJson))
        {
            foreach (var property in stored.RootElement.EnumerateObject().Where(p => !byKey.ContainsKey(p.Name)))
            {
                values[property.Name] = property.Value.Clone();
            }
        }

        foreach (var definition in active)
        {
            input.TryGetValue(definition.Key, out var element);
            var (value, error) = Parse(definition, element);
            if (error is not null)
            {
                errors[Prefix + definition.Key] = [error];
            }
            else if (value is null || (definition.Type == CustomFieldType.Checkbox && definition.Required && value is false))
            {
                if (definition.Required)
                {
                    errors[Prefix + definition.Key] = ["This field is required."];
                }
            }
            else
            {
                values[definition.Key] = value;
            }
        }

        return errors.Count > 0 ? Result<string>.Failure(CrmErrors.Invalid(errors)) : JsonSerializer.Serialize(values);
    }

    /// <summary>The stored values of the active definitions only.</summary>
    public static IReadOnlyDictionary<string, JsonElement> ForRead(
        string storedJson, IReadOnlyList<CustomFieldDefinitionInfo> active)
    {
        var result = new Dictionary<string, JsonElement>();
        if (active.Count == 0 || storedJson == "{}")
        {
            return result;
        }

        var keys = active.Select(d => d.Key).ToHashSet();
        using var document = JsonDocument.Parse(storedJson);
        foreach (var property in document.RootElement.EnumerateObject().Where(p => keys.Contains(p.Name)))
        {
            result[property.Name] = property.Value.Clone();
        }

        return result;
    }

    private static (object? Value, string? Error) Parse(CustomFieldDefinitionInfo definition, JsonElement element)
    {
        if (element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            return (null, null);
        }

        switch (definition.Type)
        {
            case CustomFieldType.Text when element.ValueKind == JsonValueKind.String:
                var text = element.GetString()!.Trim();
                return text.Length > MaxTextLength
                    ? (null, $"Text can be at most {MaxTextLength} characters.")
                    : (text.Length == 0 ? null : text, null);
            case CustomFieldType.Number when element.ValueKind == JsonValueKind.Number:
                return element.TryGetDecimal(out var number) ? (number, null) : (null, "Enter a valid number.");
            case CustomFieldType.Date when element.ValueKind == JsonValueKind.String:
                var date = element.GetString()!;
                return DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                    ? (date, null)
                    : (null, "Enter a date as yyyy-MM-dd.");
            case CustomFieldType.Select when element.ValueKind == JsonValueKind.String:
                var option = element.GetString()!;
                return option.Length == 0 ? (null, null)
                    : definition.Options.Contains(option) ? (option, null) : (null, "Choose one of the listed options.");
            case CustomFieldType.Checkbox when element.ValueKind is JsonValueKind.True or JsonValueKind.False:
                return (element.ValueKind == JsonValueKind.True, null);
            default:
                return (null, $"Expected a {definition.Type.ToString().ToLowerInvariant()} value.");
        }
    }
}
