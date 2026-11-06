using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sync;

public sealed record UpsertUserCommand(
    string ClerkUserId,
    string? Email,
    string? FirstName,
    string? LastName,
    string? ImageUrl) : ICommand<Result>;
