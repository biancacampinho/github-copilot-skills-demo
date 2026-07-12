using FluentValidation;

namespace MicroDemo.Application.Features.Prices.Commands.CreatePrice;

public class CreatePriceCommandValidator : AbstractValidator<CreatePriceCommand>
{
    public CreatePriceCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("O produto é obrigatório.");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("O valor não pode ser negativo.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3).WithMessage("A moeda deve ser um código ISO-4217 de 3 letras.");
    }
}
