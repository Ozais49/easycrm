using System.Text.Json;

namespace Cem.Identity.Endpoints.Webhooks;

public sealed record ClerkWebhookEnvelope(string? Type, JsonElement Data);
