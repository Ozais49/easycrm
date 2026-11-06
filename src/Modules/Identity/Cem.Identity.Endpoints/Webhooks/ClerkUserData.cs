namespace Cem.Identity.Endpoints.Webhooks;

public sealed record ClerkUserData(
    string? Id,
    string? FirstName,
    string? LastName,
    string? ImageUrl,
    string? PrimaryEmailAddressId,
    ClerkEmailAddress[]? EmailAddresses)
{
    public string? PrimaryEmail =>
        EmailAddresses?.FirstOrDefault(e => e.Id == PrimaryEmailAddressId)?.EmailAddress;
}
