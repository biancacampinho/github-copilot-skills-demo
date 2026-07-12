using FluentValidation;

namespace MicroDemo.Application.Features.Orders.Commands.CreateOrder;

/// <summary>
/// Validação de forma (shape) do request. As regras de NEGÓCIO cruzadas
/// (user existe? produto ativo? preço corrente? cálculo do total?) ficam no
/// handler, pois dependem de acesso ao banco de dados.
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("O pedido deve conter pelo menos um item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
        });
    }
}
