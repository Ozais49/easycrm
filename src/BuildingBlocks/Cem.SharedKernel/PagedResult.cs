namespace Cem.SharedKernel;

/// <summary>One page of a list response: <c>{ items, page, pageSize, total }</c>.</summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total);
