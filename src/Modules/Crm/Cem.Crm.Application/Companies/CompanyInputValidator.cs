using System.Text.RegularExpressions;
using Cem.Crm.Application.Tags;
using FluentValidation;

namespace Cem.Crm.Application.Companies;

public abstract partial class CompanyInputValidator<T> : AbstractValidator<T> where T : ICompanyInput
{
    protected CompanyInputValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Industry).MaximumLength(100);
        RuleFor(c => c.Website).MaximumLength(300);
        RuleFor(c => c.Phone).MaximumLength(30);
        RuleFor(c => c.Email).MaximumLength(254).EmailAddress().When(c => !string.IsNullOrWhiteSpace(c.Email));
        RuleFor(c => c.PanNumber).Matches(NineDigits()).When(c => !string.IsNullOrWhiteSpace(c.PanNumber))
            .WithMessage("PAN number must be exactly 9 digits.");
        RuleFor(c => c.Address!).SetValidator(new AddressValidator()).When(c => c.Address is not null);
        this.TagListRules(c => c.Tags);
    }

    [GeneratedRegex("^[0-9]{9}$")]
    private static partial Regex NineDigits();
}
