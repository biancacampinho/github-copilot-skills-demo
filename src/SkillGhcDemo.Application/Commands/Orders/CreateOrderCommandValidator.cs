using FluentValidation;

namespace SkillGhcDemo.Application.Commands.Orders;

/// <summary>
/// Shape validation of the request. The cross-cutting BUSINESS rules
/// (does the user exist? is the product active? current price? total calculation?) live in the
/// handler, since they depend on database access.
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("The order must contain at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("The quantity must be greater than zero.");
        });
    }
}
