using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Contacts;

/// <param name="OwnerUserId">Null keeps the current owner.</param>
public sealed record UpdateContactCommand(
    Guid Id,
    string FirstName,
    string? LastName,
    string? JobTitle,
    Guid? CompanyId,
    string? Email,
    string? Phone,
    string? Mobile,
    AddressDto? Address,
    Guid? OwnerUserId,
    Guid? TeamId) : ICommand<Result<ContactDto>>, IContactInput;
