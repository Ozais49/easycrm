using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.Tenant;

public sealed record GetTenantSettingsQuery : IQuery<Result<TenantSettingsDto>>;
