using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Sync;

public sealed record DeleteUserCommand(string ClerkUserId) : ICommand<Result>;
