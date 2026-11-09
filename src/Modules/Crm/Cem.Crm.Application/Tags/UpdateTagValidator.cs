using FluentValidation;

namespace Cem.Crm.Application.Tags;

public sealed class UpdateTagValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(TagNames.MaxLength);
        RuleFor(c => c.Color).IsInEnum();
    }
}
