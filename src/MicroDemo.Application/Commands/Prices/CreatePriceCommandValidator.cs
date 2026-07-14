using FluentValidation;

namespace MicroDemo.Application.Commands.Prices;

public class CreatePriceCommandValidator : AbstractValidator<CreatePriceCommand>
{
    public CreatePriceCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("The product is required.");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("The amount cannot be negative.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3).WithMessage("The currency must be a 3-letter ISO-4217 code.");
    }
}
