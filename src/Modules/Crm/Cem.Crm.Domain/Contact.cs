using Cem.SharedKernel;
using Cem.SharedKernel.Permissions;

namespace Cem.Crm.Domain;

/// <summary>A person; belongs to at most one company. The same person at two companies is two records.</summary>
public sealed class Contact : AuditableEntity, ITenantEntity, ISoftDeletable, IOwnedRecord
{
    private Contact()
    {
    }

    public Guid TenantId { get; private set; }
    public string FirstName { get; private set; } = "";
    public string? LastName { get; private set; }
    public string? JobTitle { get; private set; }
    public Guid? CompanyId { get; private set; }

    // Contact PII: tagged so the audit log records that it changed, never the value.
    [Sensitive(FieldGroup.ContactPii)]
    public string? Email { get; private set; }

    [Sensitive(FieldGroup.ContactPii)]
    public string? Phone { get; private set; }

    [Sensitive(FieldGroup.ContactPii)]
    public string? Mobile { get; private set; }

    [Sensitive(FieldGroup.ContactPii)]
    public Address Address { get; private set; } = new();

    public Guid? OwnerUserId { get; private set; }
    public Guid? TeamId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    /// <summary>Admin-defined field values as a JSON object keyed by field key (jsonb).</summary>
    public string CustomFields { get; private set; } = "{}";

    public static Contact Create(
        string firstName, string? lastName, string? jobTitle, Guid? companyId,
        string? email, string? phone, string? mobile, Address address, Guid? ownerUserId, Guid? teamId)
    {
        var contact = new Contact { OwnerUserId = ownerUserId, TeamId = teamId };
        contact.Update(firstName, lastName, jobTitle, companyId);
        contact.SetContactDetails(email, phone, mobile, address);
        return contact;
    }

    public void Update(string firstName, string? lastName, string? jobTitle, Guid? companyId)
    {
        FirstName = firstName;
        LastName = lastName;
        JobTitle = jobTitle;
        CompanyId = companyId;
    }

    /// <summary>The PII part; separate because only callers with ContactPii edit access may change it.</summary>
    public void SetContactDetails(string? email, string? phone, string? mobile, Address address)
    {
        Email = email;
        Phone = phone;
        Mobile = mobile;
        Address = address;
    }

    public void Assign(Guid? ownerUserId, Guid? teamId)
    {
        OwnerUserId = ownerUserId;
        TeamId = teamId;
    }

    public void SetCustomFields(string json) => CustomFields = json;

    public void MarkDeleted(DateTimeOffset now) => DeletedAt ??= now;
}
