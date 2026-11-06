using Cem.SharedKernel.Permissions;

namespace Cem.Identity.Application.Permissions;

/// <summary>What the permission service caches per user: effective permissions and team ids.</summary>
public sealed record PermissionSnapshot(PermissionMatrix Matrix, IReadOnlyCollection<Guid> TeamIds);
