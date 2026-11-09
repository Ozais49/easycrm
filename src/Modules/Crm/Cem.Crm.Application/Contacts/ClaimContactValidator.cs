using FluentValidation;

namespace Cem.Crm.Application.Contacts;

public sealed class ClaimContactValidator : AbstractValidator<ClaimContactCommand>
{
    public ClaimContactValidator() => RuleFor(c => c.Id).NotEmpty();
}
