namespace Cem.Identity.Application.Abstractions;

/// <summary>Drops the cached permissions of every user in the current tenant. Call after saving role, assignment or team changes.</summary>
public interface IPermissionCacheInvalidator
{
    void InvalidateTenant();
}
