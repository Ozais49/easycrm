namespace Cem.Identity.Application.Me;

public sealed record MeUserDto(Guid Id, string? Email, string? FirstName, string? LastName, string? ImageUrl);
