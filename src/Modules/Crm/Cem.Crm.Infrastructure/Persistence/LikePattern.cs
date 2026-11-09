namespace Cem.Crm.Infrastructure.Persistence;

internal static class LikePattern
{
    /// <summary>A contains-pattern for ILIKE with the wildcard characters of the input escaped.</summary>
    public static string Contains(string text) =>
        "%" + text.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";
}
