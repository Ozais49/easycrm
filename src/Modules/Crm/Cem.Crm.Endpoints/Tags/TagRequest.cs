using Cem.Crm.Domain;

namespace Cem.Crm.Endpoints.Tags;

/// <summary>Body for renaming and recoloring a tag.</summary>
public sealed record TagRequest(string Name, TagColor Color);
