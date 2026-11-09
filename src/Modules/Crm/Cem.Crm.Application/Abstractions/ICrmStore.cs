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

    /// <summary>
    /// The tags with these names (case-insensitive); names without a tag get a new one (neutral, saved with the next
    /// SaveChanges). The result has one tag per name, in the order of the names.
    /// </summary>
    Task<IReadOnlyList<Tag>> ResolveTagsAsync(IReadOnlyList<string> names, CancellationToken cancellationToken);

    /// <summary>Makes the tags of the company exactly these; applied by the next SaveChanges.</summary>
    Task SetTagsAsync(Company company, IReadOnlyList<Tag> tags, CancellationToken cancellationToken);

    /// <summary>Makes the tags of the contact exactly these; applied by the next SaveChanges.</summary>
    Task SetTagsAsync(Contact contact, IReadOnlyList<Tag> tags, CancellationToken cancellationToken);

    Task<Tag?> FindTagAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>True when another tag (not <paramref name="exceptId"/>) has this name, ignoring case.</summary>
    Task<bool> TagNameTakenAsync(string name, Guid exceptId, CancellationToken cancellationToken);

    void Remove(Tag tag);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Saves, returning false when a unique index rejected the change (a tag name taken concurrently); the rejected
    /// inserts are dropped from the tracker.
    /// </summary>
    Task<bool> TrySaveAsync(CancellationToken cancellationToken);
}
