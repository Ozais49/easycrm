namespace Cem.Infrastructure.Common.Auth;

/// <summary>The <c>Clerk</c> configuration section.</summary>
public sealed class ClerkOptions
{
    public const string SectionName = "Clerk";

    /// <summary>Clerk Frontend API URL, e.g. <c>https://example.clerk.accounts.dev</c>. Token issuer and JWKS discovery root.</summary>
    public string Authority { get; init; } = "";

    /// <summary>Allowed values of the token's <c>azp</c> claim: the origins of our web app.</summary>
    public string[] AuthorizedParties { get; init; } = [];

    /// <summary>Svix signing secret (<c>whsec_...</c>) of the Clerk webhook endpoint.</summary>
    public string WebhookSigningSecret { get; init; } = "";

    public bool IsComplete =>
        Authority.Length > 0 && AuthorizedParties.Length > 0 && WebhookSigningSecret.Length > 0;
}
