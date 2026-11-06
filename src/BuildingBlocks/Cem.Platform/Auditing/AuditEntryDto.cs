using System.Text.Json;
using Cem.Infrastructure.Common.Auditing;

namespace Cem.Platform.Auditing;

public sealed record AuditEntryDto(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid? UserId,
    AuditAction Action,
    string EntityType,
    Guid EntityId,
    JsonElement Changes);
