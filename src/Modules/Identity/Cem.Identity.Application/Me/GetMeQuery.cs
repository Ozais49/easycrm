using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Me;

/// <summary>The current user, tenant, roles, teams and effective permissions.</summary>
public sealed record GetMeQuery : IQuery<Result<MeDto>>;
