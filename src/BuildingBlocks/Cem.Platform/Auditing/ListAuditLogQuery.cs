using Cem.SharedKernel;
using Mediator;

namespace Cem.Platform.Auditing;

/// <param name="EntityType">Schema and table, e.g. <c>identity.roles</c>.</param>
public sealed record ListAuditLogQuery(string? EntityType, Guid? EntityId, Guid? UserId, int Page, int PageSize)
    : IQuery<Result<PagedResult<AuditEntryDto>>>;
