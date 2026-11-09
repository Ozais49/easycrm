namespace Cem.Crm.Application;

internal static class Text
{
    /// <summary>Trimmed text; blank becomes null so an empty form field never stores an empty string.</summary>
    public static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
