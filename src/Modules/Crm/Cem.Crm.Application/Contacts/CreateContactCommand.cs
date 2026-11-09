using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Contacts;

/// <param name="OwnerUserId">Defaults to the current user.</param>
public sealed record CreateContactCommand(
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
