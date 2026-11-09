using Cem.Crm.Application.Tags;
using FluentValidation;

namespace Cem.Crm.Application.Contacts;

public abstract class ContactInputValidator<T> : AbstractValidator<T> where T : IContactInput
{
    protected ContactInputValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(c => c.LastName).MaximumLength(100);
        RuleFor(c => c.JobTitle).MaximumLength(100);
        RuleFor(c => c.Email).MaximumLength(254).EmailAddress().When(c => !string.IsNullOrWhiteSpace(c.Email));
        RuleFor(c => c.Phone).MaximumLength(30);
        RuleFor(c => c.Mobile).MaximumLength(30);
        RuleFor(c => c.Address!).SetValidator(new AddressValidator()).When(c => c.Address is not null);
        this.TagListRules(c => c.Tags);
    }
}
