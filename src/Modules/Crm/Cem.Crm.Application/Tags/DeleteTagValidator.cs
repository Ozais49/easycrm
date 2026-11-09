using FluentValidation;

namespace Cem.Crm.Application.Tags;

public sealed class DeleteTagValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagValidator() => RuleFor(c => c.Id).NotEmpty();
}
