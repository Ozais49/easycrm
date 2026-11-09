using FluentValidation;

namespace Cem.Crm.Application.Contacts;

public sealed class GetContactValidator : AbstractValidator<GetContactQuery>
{
    public GetContactValidator() => RuleFor(q => q.Id).NotEmpty();
}
