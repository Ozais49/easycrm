using Cem.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Cem.Infrastructure.Common.Persistence;

public static class QueryableExtensions
{
    /// <summary>Runs the (already ordered, projected) query as one page plus the total count.</summary>
    public static async Task<PagedResult<T>> ToPagedAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<T>(items, page, pageSize, total);
    }
}
