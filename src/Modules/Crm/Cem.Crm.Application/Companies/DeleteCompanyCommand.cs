using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Companies;

public sealed record DeleteCompanyCommand(Guid Id) : ICommand<Result>;
