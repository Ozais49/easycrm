using Cem.SharedKernel;

namespace Cem.Crm.Domain;

/// <summary>A client organization.</summary>
public sealed class Company : AuditableEntity, ITenantEntity, ISoftDeletable, IOwnedRecord
{
    private Company()
    {
    }

    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = "";
    public string? Industry { get; private set; }
    public string? Website { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }

    /// <summary>Nepal PAN / VAT number: exactly nine digits.</summary>
    public string? PanNumber { get; private set; }

    public Address Address { get; private set; } = new();
    public Guid OwnerUserId { get; private set; }
    public Guid? TeamId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    /// <summary>Admin-defined field values as a JSON object keyed by field key (jsonb).</summary>
    public string CustomFields { get; private set; } = "{}";

    public static Company Create(
        string name, string? industry, string? website, string? phone, string? email, string? panNumber,
        Address address, Guid ownerUserId, Guid? teamId)
    {
        var company = new Company { OwnerUserId = ownerUserId, TeamId = teamId };
        company.Update(name, industry, website, phone, email, panNumber, address);
        return company;
    }

    public void Update(
        string name, string? industry, string? website, string? phone, string? email, string? panNumber, Address address)
    {
        Name = name;
        Industry = industry;
        Website = website;
        Phone = phone;
        Email = email;
        PanNumber = panNumber;
        Address = address;
    }

    public void Assign(Guid ownerUserId, Guid? teamId)
    {
        OwnerUserId = ownerUserId;
        TeamId = teamId;
    }

    public void SetCustomFields(string json) => CustomFields = json;

    public void MarkDeleted(DateTimeOffset now) => DeletedAt ??= now;
}
