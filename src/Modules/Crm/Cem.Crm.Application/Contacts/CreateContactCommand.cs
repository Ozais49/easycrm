using System.Text.Json;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Contacts;

/// <param name="OwnerUserId">Null leaves the record unassigned (the web form defaults to the current user).</param>
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
    Guid? TeamId,
    IReadOnlyList<string>? Tags,
    IReadOnlyDictionary<string, JsonElement>? CustomFields) : ICommand<Result<ContactDto>>, IContactInput;
