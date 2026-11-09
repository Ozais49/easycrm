namespace Cem.Crm.Application.Companies;

public sealed record CompanyDto(
    Guid Id,
    string Name,
    string? Industry,
    string? Website,
    string? Phone,
    string? Email,
    string? PanNumber,
    AddressDto Address,
    Guid OwnerUserId,
    Guid? TeamId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
