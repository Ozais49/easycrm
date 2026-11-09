using Cem.SharedKernel;

namespace Cem.Crm.Domain;

/// <summary>Assignment of a tag to a company; the key is (company, tag).</summary>
public sealed class CompanyTag : ITenantEntity
{
    private CompanyTag()
    {
    }

    public Guid TenantId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid TagId { get; private set; }

    public static CompanyTag Create(Guid companyId, Guid tagId) => new() { CompanyId = companyId, TagId = tagId };
}
