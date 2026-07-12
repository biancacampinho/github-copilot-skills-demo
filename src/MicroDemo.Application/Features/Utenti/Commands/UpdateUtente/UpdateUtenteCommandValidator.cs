using FluentValidation;

namespace MicroDemo.Application.Features.Utenti.Commands.UpdateUtente;

public class UpdateUtenteCommandValidator : AbstractValidator<UpdateUtenteCommand>
{
    public UpdateUtenteCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
    }
}
