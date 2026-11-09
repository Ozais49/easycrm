using FluentValidation;

namespace Cem.Crm.Application.Contacts;

public sealed class UpdateContactValidator : ContactInputValidator<UpdateContactCommand>
{
    public UpdateContactValidator() => RuleFor(c => c.Id).NotEmpty();
}
