namespace Cem.Crm.Domain;

/// <summary>A record whose visibility follows the "own" / "team" permission levels.</summary>
public interface IOwnedRecord
{
    /// <summary>Null: unassigned, waiting for someone to take it up (see RecordAccess).</summary>
    Guid? OwnerUserId { get; }

    Guid? TeamId { get; }
}
