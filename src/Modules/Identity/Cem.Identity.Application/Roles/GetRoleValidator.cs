using FluentValidation;

namespace Cem.Identity.Application.Roles;

public sealed class GetRoleValidator : AbstractValidator<GetRoleQuery>
{
    public GetRoleValidator() => RuleFor(q => q.RoleId).NotEmpty();
}
