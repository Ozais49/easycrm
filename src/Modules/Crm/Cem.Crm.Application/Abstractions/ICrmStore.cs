using Cem.Crm.Domain;

namespace Cem.Crm.Application.Abstractions;

/// <summary>Write-side port of the Crm module; everything is looked up within the current tenant.</summary>
public interface ICrmStore
{
    /// <summary>A company of the current tenant, tracked for update; null when missing or deleted.</summary>
    Task<Company?> FindCompanyAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> CompanyExistsAsync(Guid id, CancellationToken cancellationToken);

    Task<Contact?> FindContactAsync(Guid id, CancellationToken cancellationToken);

    void Add(Company company);

    void Add(Contact contact);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
