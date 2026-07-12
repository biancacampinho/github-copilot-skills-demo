using FluentValidation;

namespace MicroDemo.Application.Features.Prices.Commands.CreatePrice;

public class CreatePriceCommandValidator : AbstractValidator<CreatePriceCommand>
{
    public CreatePriceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do preço é obrigatório.")
            .MaximumLength(120);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("O valor não pode ser negativo.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3).WithMessage("A moeda deve ser um código ISO-4217 de 3 letras.");

        RuleFor(x => x.BillingPeriod)
            .IsInEnum();
    }
}
