using FluentValidation;

namespace Cem.Crm.Application;

public sealed class AddressValidator : AbstractValidator<AddressDto>
{
    public AddressValidator()
    {
        RuleFor(a => a.Street).MaximumLength(200);
        RuleFor(a => a.City).MaximumLength(100);
        RuleFor(a => a.District).MaximumLength(100);
        RuleFor(a => a.Province).IsInEnum();
    }
}
