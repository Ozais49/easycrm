using Cem.Settings.Application.Abstractions;
using Cem.Settings.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.Tenant;

public sealed class UpdateTenantSettingsHandler(ISettingsStore store)
    : ICommandHandler<UpdateTenantSettingsCommand, Result<TenantSettingsDto>>
{
    public async ValueTask<Result<TenantSettingsDto>> Handle(
        UpdateTenantSettingsCommand command, CancellationToken cancellationToken)
    {
        var settings = await store.FindAsync(cancellationToken);
        if (settings is null)
        {
            settings = TenantSettings.CreateDefault();
            settings.Update(command.TimeZone, command.DateDisplay, command.BrandColor);
            store.Add(settings);

            if (await store.TrySaveNewAsync(cancellationToken))
            {
                return TenantSettingsDto.From(settings);
            }

            // A concurrent request (or the provisioning event) created the row first: update that one.
            settings = await store.FindAsync(cancellationToken)
                ?? throw new InvalidOperationException("Tenant settings vanished after a unique violation.");
        }

        settings.Update(command.TimeZone, command.DateDisplay, command.BrandColor);
        await store.SaveChangesAsync(cancellationToken);
        return TenantSettingsDto.From(settings);
    }
}
