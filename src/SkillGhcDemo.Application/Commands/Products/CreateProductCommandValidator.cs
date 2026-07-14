using FluentValidation;

namespace SkillGhcDemo.Application.Commands.Products;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The product name is required.")
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("The SKU is required.")
            .MaximumLength(50);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("The category is required.");
    }
}
