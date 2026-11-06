namespace Cem.SharedKernel;

/// <summary>
/// Establishes the tenant and user for the current scope. Reserved for identity resolution
/// (session middleware, webhooks); feature code only reads <see cref="ITenantContext"/> and <see cref="ICurrentUser"/>.
/// </summary>
public interface IRequestContextSetter
{
    /// <summary>Idempotent for the same tenant; throws if the scope already belongs to a different one.</summary>
    void SetTenant(Guid tenantId);

    /// <summary>Idempotent for the same user; throws if the scope already belongs to a different one.</summary>
    void SetUser(Guid userId, string clerkUserId);
}
