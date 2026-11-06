using Cem.SharedKernel;

namespace Cem.Identity.Domain;

/// <summary>A group of members; "team" permission levels reach the records of the user's teams.</summary>
public sealed class Team : AuditableEntity, ITenantEntity, ISoftDeletable
{
    private Team()
    {
    }

    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = "";
    public DateTimeOffset? DeletedAt { get; private set; }

    public static Team Create(string name) => new() { Name = name };

    public void Rename(string name) => Name = name;

    public void MarkDeleted(DateTimeOffset now) => DeletedAt ??= now;
}
