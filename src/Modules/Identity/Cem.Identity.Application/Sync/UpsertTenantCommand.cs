using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sync;

public sealed record UpsertTenantCommand(string ClerkOrgId, string Name, string Slug) : ICommand<Result>;
