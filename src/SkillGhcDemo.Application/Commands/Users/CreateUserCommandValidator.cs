using FluentValidation;

namespace SkillGhcDemo.Application.Commands.Users;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("The user's name is required.")
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("The e-mail is required.")
            .EmailAddress().WithMessage("Invalid e-mail.")
            .MaximumLength(256);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30);
    }
}
