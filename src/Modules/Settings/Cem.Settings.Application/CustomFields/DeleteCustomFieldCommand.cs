using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.CustomFields;

public sealed record DeleteCustomFieldCommand(Guid Id) : ICommand<Result>;
