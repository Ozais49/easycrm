using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sync;

public sealed record RemoveMembershipCommand(string ClerkOrgId, string ClerkUserId) : ICommand<Result>;
