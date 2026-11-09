using System.Text.Json;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Contacts;

/// <param name="OwnerUserId">Null unassigns the record.</param>
/// <param name="Tags">Replaces the tags of the contact; null keeps them.</param>
/// <param name="CustomFields">Replaces the values of the active custom fields; null keeps all values.</param>
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
    Guid? TeamId,
    IReadOnlyList<string>? Tags,
    IReadOnlyDictionary<string, JsonElement>? CustomFields) : ICommand<Result<ContactDto>>, IContactInput;
