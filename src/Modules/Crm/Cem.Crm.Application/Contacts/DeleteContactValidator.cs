using FluentValidation;

namespace Cem.Crm.Application.Contacts;

public sealed class DeleteContactValidator : AbstractValidator<DeleteContactCommand>
{
    public DeleteContactValidator() => RuleFor(c => c.Id).NotEmpty();
}
