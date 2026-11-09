using FluentValidation;

namespace Cem.Settings.Application.CustomFields;

public sealed class DeleteCustomFieldValidator : AbstractValidator<DeleteCustomFieldCommand>
{
    public DeleteCustomFieldValidator() => RuleFor(c => c.Id).NotEmpty();
}
