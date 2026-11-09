using Cem.SharedKernel;

namespace Cem.Settings.Domain;

/// <summary>
/// An admin-defined field on companies or contacts of a tenant. Values live in the record's <c>custom_fields</c>
/// jsonb under <see cref="Key"/>. Key and type never change after creation; deleting hides the definition only.
/// </summary>
public sealed class CustomFieldDefinition : AuditableEntity, ITenantEntity, ISoftDeletable
{
    private CustomFieldDefinition()
    {
    }

    public Guid TenantId { get; private set; }
    public CustomFieldEntity EntityType { get; private set; }

    /// <summary>Slug <c>[a-z][a-z0-9_]{0,39}</c>, unique per tenant and entity type among active definitions.</summary>
    public string Key { get; private set; } = "";

    public string Label { get; private set; } = "";
    public CustomFieldType Type { get; private set; }
    public bool Required { get; private set; }

    /// <summary>Allowed values of a select field; empty for every other type.</summary>
    public string[] Options { get; private set; } = [];

    public int SortOrder { get; private set; }
    public bool ShowInList { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public static CustomFieldDefinition Create(
        CustomFieldEntity entityType, string key, string label, CustomFieldType type, bool required,
        string[] options, bool showInList, int sortOrder) => new()
    {
        EntityType = entityType,
        Key = key,
        Type = type,
        SortOrder = sortOrder,
        Label = label,
        Required = required,
        Options = options,
        ShowInList = showInList
    };

    public void Update(string label, bool required, string[] options, bool showInList)
    {
        Label = label;
        Required = required;
        Options = options;
        ShowInList = showInList;
    }

    public void MoveTo(int sortOrder) => SortOrder = sortOrder;

    public void MarkDeleted(DateTimeOffset now) => DeletedAt ??= now;
}
