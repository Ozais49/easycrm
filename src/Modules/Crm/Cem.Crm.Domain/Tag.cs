using Cem.SharedKernel;

namespace Cem.Crm.Domain;

/// <summary>A free-form label shared by the companies and contacts of a tenant; the name is unique per tenant ignoring case.</summary>
public sealed class Tag : AuditableEntity, ITenantEntity
{
    private Tag()
    {
    }

    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = "";
    public TagColor Color { get; private set; }

    public static Tag Create(string name) => new() { Name = name, Color = TagColor.Neutral };

    public void Update(string name, TagColor color)
    {
        Name = name;
        Color = color;
    }
}
