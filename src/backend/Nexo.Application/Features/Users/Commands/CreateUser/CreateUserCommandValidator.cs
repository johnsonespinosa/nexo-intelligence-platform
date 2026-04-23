using FluentValidation;

namespace Nexo.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
        RuleFor(x => x.Password).MinimumLength(6).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Role).Must(r => r is "User" or "Manager" or "Admin");
        RuleFor(x => x.Tier).Must(t => t is "BestEffort" or "Standard" or "Enterprise");
    }
}
