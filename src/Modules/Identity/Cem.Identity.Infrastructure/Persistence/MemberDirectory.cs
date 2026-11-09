using Cem.Identity.Contracts;
using Cem.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace Cem.Identity.Infrastructure.Persistence;

public sealed class MemberDirectory(IdentityDbContext db) : IMemberDirectory
{
    public Task<bool> IsActiveMemberAsync(Guid userId, CancellationToken cancellationToken) =>
        db.TenantMemberships.AsNoTracking()
            .AnyAsync(m => m.UserId == userId && m.Status == MembershipStatus.Active, cancellationToken);

    public Task<bool> TeamExistsAsync(Guid teamId, CancellationToken cancellationToken) =>
        db.Teams.AsNoTracking().AnyAsync(t => t.Id == teamId, cancellationToken);
}
