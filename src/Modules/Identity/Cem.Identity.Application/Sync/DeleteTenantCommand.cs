using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sync;

public sealed record DeleteTenantCommand(string ClerkOrgId) : ICommand<Result>;
