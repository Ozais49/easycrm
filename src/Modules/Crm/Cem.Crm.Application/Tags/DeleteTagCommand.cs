using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Tags;

public sealed record DeleteTagCommand(Guid Id) : ICommand<Result>;
