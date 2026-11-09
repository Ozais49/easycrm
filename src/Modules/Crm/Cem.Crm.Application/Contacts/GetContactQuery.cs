using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Contacts;

public sealed record GetContactQuery(Guid Id) : IQuery<Result<ContactDto>>;
