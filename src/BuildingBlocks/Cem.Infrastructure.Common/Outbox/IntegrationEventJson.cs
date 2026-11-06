using System.Text.Json;

namespace Cem.Infrastructure.Common.Outbox;

/// <summary>The one JSON format of outbox payloads: written when an event is published, read when it is delivered.</summary>
public static class IntegrationEventJson
{
    public static JsonSerializerOptions Options { get; } = new(JsonSerializerDefaults.Web);
}
