using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sessions;

/// <summary>Resolves (and just-in-time provisions) the tenant, user and membership behind a Clerk session token.</summary>
public sealed record ResolveSessionCommand(string ClerkUserId, string ClerkOrgId, string? OrgSlug, bool IsOrgAdmin)
    : ICommand<Result<SessionInfo>>;
