using Cem.Settings.Application.Abstractions;
using Cem.Settings.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.Tenant;

public sealed class GetTenantSettingsHandler(ISettingsStore store)
    : IQueryHandler<GetTenantSettingsQuery, Result<TenantSettingsDto>>
{
    public async ValueTask<Result<TenantSettingsDto>> Handle(GetTenantSettingsQuery query, CancellationToken cancellationToken)
    {
        // No row yet (event not processed, or the tenant predates this feature): answer with the defaults, write nothing.
        var settings = await store.FindReadOnlyAsync(cancellationToken) ?? TenantSettings.CreateDefault();
        return TenantSettingsDto.From(settings);
    }
}
