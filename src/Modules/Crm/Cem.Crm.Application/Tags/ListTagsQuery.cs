using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Tags;

public sealed record ListTagsQuery : IQuery<Result<IReadOnlyList<TagDto>>>;
