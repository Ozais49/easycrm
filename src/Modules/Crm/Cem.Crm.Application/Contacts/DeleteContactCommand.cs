using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Contacts;

public sealed record DeleteContactCommand(Guid Id) : ICommand<Result>;
