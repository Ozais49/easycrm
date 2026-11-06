namespace Cem.SharedKernel;

/// <summary>
/// A fact one module publishes for others to react to, delivered through the outbox (ADR-0005, ADR-0017).
/// Implement it on an immutable record named in the past tense; the payload must stay serializable by System.Text.Json.
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>
    /// Stable wire name, e.g. <c>identity.tenant_created</c>. It is stored with every message, so renaming or moving
    /// the class never breaks messages already in the outbox; never change a published value.
    /// </summary>
    static abstract string EventType { get; }
}
