using Cem.SharedKernel;

namespace Cem.Crm.Domain;

/// <summary>Assignment of a tag to a contact; the key is (contact, tag).</summary>
public sealed class ContactTag : ITenantEntity
{
    private ContactTag()
    {
    }

    public Guid TenantId { get; private set; }
    public Guid ContactId { get; private set; }
    public Guid TagId { get; private set; }

    public static ContactTag Create(Guid contactId, Guid tagId) => new() { ContactId = contactId, TagId = tagId };
}
