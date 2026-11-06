using Cem.Infrastructure.Common.Auditing;
using Cem.Infrastructure.Common.Persistence;
using Cem.Platform.Persistence;
using Cem.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Cem.Platform.Auditing;

/// <summary>Newest first. The tenant filter of <see cref="AuditEntry"/> limits the result to the current tenant.</summary>
public sealed class ListAuditLogHandler(PlatformDbContext db)
    : IQueryHandler<ListAuditLogQuery, Result<PagedResult<AuditEntryDto>>>
{
    public async ValueTask<Result<PagedResult<AuditEntryDto>>> Handle(ListAuditLogQuery query, CancellationToken cancellationToken)
    {
        var entries = db.AuditEntries.AsNoTracking();

        if (query.EntityType is not null)
        {
            entries = entries.Where(e => e.EntityType == query.EntityType);
        }

        if (query.EntityId is { } entityId)
        {
            entries = entries.Where(e => e.EntityId == entityId);
        }

        if (query.UserId is { } userId)
        {
            entries = entries.Where(e => e.UserId == userId);
        }

        return await entries
            .OrderByDescending(e => e.OccurredAt)
            .ThenByDescending(e => e.Id)
            .Select(e => new AuditEntryDto(e.Id, e.OccurredAt, e.UserId, e.Action, e.EntityType, e.EntityId, e.Changes))
            .ToPagedAsync(query.Page, query.PageSize, cancellationToken);
    }
}
