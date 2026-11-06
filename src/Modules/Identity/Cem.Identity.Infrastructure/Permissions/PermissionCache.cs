using Cem.Identity.Application.Abstractions;
using Cem.Identity.Application.Permissions;
using Cem.SharedKernel;
using Microsoft.Extensions.Caching.Memory;

namespace Cem.Identity.Infrastructure.Permissions;

/// <summary>
/// Short-lived cache of permission snapshots per (tenant, user), in the shared bounded memory cache.
/// Invalidation is per tenant: each tenant has a version token that is part of every snapshot key, and
/// invalidating swaps in a new token, which orphans the old entries (they expire on their own).
/// A snapshot loaded just before an invalidation is stored under the old token, so it is never served afterwards.
/// </summary>
public sealed class PermissionCache(IMemoryCache cache, ITenantContext tenantContext, ICurrentUser currentUser)
    : IPermissionCacheInvalidator
{
    private static readonly TimeSpan SnapshotTtl = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan VersionTtl = TimeSpan.FromHours(1);

    public async ValueTask<PermissionSnapshot> GetOrLoadAsync(Func<Task<PermissionSnapshot>> load)
    {
        var tenantId = tenantContext.TenantId!.Value;
        var key = $"permissions:{tenantId}:{GetVersion(tenantId)}:{currentUser.UserId}";

        if (cache.TryGetValue(key, out PermissionSnapshot? snapshot) && snapshot is not null)
        {
            return snapshot;
        }

        snapshot = await load();
        cache.Set(key, snapshot, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = SnapshotTtl, Size = 1 });
        return snapshot;
    }

    public void InvalidateTenant() => SetVersion(tenantContext.TenantId!.Value, Guid.NewGuid());

    private Guid GetVersion(Guid tenantId)
    {
        if (cache.TryGetValue(VersionKey(tenantId), out Guid version))
        {
            return version;
        }

        version = Guid.NewGuid();
        SetVersion(tenantId, version);
        return version;
    }

    private void SetVersion(Guid tenantId, Guid version) =>
        cache.Set(VersionKey(tenantId), version, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = VersionTtl, Size = 1 });

    private static string VersionKey(Guid tenantId) => $"permissions-version:{tenantId}";
}
