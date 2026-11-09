using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Contacts;

/// <summary>Takes up an unassigned contact: the caller becomes its owner.</summary>
public sealed record ClaimContactCommand(Guid Id) : ICommand<Result<ContactDto>>;
