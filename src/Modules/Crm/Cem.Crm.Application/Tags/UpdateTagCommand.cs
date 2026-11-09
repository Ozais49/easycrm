using Cem.Crm.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Tags;

public sealed record UpdateTagCommand(Guid Id, string Name, TagColor Color) : ICommand<Result<TagDto>>;
