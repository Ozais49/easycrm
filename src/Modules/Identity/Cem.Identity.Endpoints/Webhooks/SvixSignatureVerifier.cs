using System.Security.Cryptography;
using System.Text;

namespace Cem.Identity.Endpoints.Webhooks;

/// <summary>Verifies Svix webhook signatures (used by Clerk) without the Svix SDK.</summary>
public static class SvixSignatureVerifier
{
    private const string SecretPrefix = "whsec_";
    private static readonly TimeSpan Tolerance = TimeSpan.FromMinutes(5);

    /// <param name="signatureHeader">Space-separated <c>v1,&lt;base64&gt;</c> entries; any one matching is enough.</param>
    public static bool IsValid(
        string secret,
        string id,
        string timestamp,
        string signatureHeader,
        string body,
        DateTimeOffset now)
    {
        if (!long.TryParse(timestamp, out var seconds))
        {
            return false;
        }

        var age = now - DateTimeOffset.FromUnixTimeSeconds(seconds);
        if (age.Duration() > Tolerance)
        {
            return false;
        }

        if (!TryDecodeSecret(secret, out var key))
        {
            return false;
        }

        var expected = HMACSHA256.HashData(key, Encoding.UTF8.GetBytes($"{id}.{timestamp}.{body}"));

        foreach (var entry in signatureHeader.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = entry.Split(',', 2);
            if (parts is ["v1", var signature] && TryDecodeBase64(signature, out var actual)
                && CryptographicOperations.FixedTimeEquals(expected, actual))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryDecodeSecret(string secret, out byte[] key)
    {
        var encoded = secret.StartsWith(SecretPrefix, StringComparison.Ordinal) ? secret[SecretPrefix.Length..] : secret;
        return TryDecodeBase64(encoded, out key) && key.Length > 0;
    }

    private static bool TryDecodeBase64(string value, out byte[] bytes)
    {
        bytes = new byte[value.Length];
        if (Convert.TryFromBase64String(value, bytes, out var written))
        {
            bytes = bytes[..written];
            return true;
        }

        bytes = [];
        return false;
    }
}
