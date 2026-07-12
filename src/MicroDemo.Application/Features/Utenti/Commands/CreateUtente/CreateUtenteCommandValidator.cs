using FluentValidation;

namespace MicroDemo.Application.Features.Utenti.Commands.CreateUtente;

public class CreateUtenteCommandValidator : AbstractValidator<CreateUtenteCommand>
{
    public CreateUtenteCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("O nome do utente é obrigatório.")
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(256);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30);
    }
}
