namespace Cem.Crm.Domain;

/// <summary>A postal address in Nepal; stored as columns of the owning row, every part optional.</summary>
public sealed record Address
{
    public string? Street { get; init; }
    public string? City { get; init; }
    public string? District { get; init; }
    public Province? Province { get; init; }
}
