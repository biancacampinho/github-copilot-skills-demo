using FluentValidation;

namespace SkillGhcDemo.Application.Commands.Categories;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The category name is required.")
            .MaximumLength(120);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
