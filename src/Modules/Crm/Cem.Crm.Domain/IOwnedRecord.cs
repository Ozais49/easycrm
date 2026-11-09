namespace Cem.Crm.Domain;

/// <summary>A record whose visibility follows the "own" / "team" permission levels.</summary>
public interface IOwnedRecord
{
    Guid OwnerUserId { get; }

    Guid? TeamId { get; }
}
