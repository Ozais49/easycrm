using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sync;

public sealed record UpsertMembershipCommand(string ClerkOrgId, string ClerkUserId, string ClerkMembershipId, bool IsOrgAdmin)
    : ICommand<Result>;
