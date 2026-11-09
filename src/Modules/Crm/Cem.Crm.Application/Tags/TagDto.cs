using Cem.Crm.Domain;

namespace Cem.Crm.Application.Tags;

public sealed record TagDto(Guid Id, string Name, TagColor Color)
{
    public static TagDto From(Tag tag) => new(tag.Id, tag.Name, tag.Color);
}
