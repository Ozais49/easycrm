using Cem.Crm.Domain;

namespace Cem.Crm.Application;

public sealed record AddressDto(string? Street, string? City, string? District, Province? Province)
{
    public static Address ToDomain(AddressDto? dto) => new()
    {
        Street = Text.Clean(dto?.Street),
        City = Text.Clean(dto?.City),
        District = Text.Clean(dto?.District),
        Province = dto?.Province
    };
}
