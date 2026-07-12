using FluentValidation;

namespace MicroDemo.Application.Features.Subscriptions.Commands.CreateSubscription;

/// <summary>
/// Validação de forma (shape) do request. As regras de NEGÓCIO cruzadas
/// (utente existe? price ativo? assinatura sobreposta?) ficam no handler,
/// pois dependem de acesso ao banco de dados.
/// </summary>
public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.UtenteId).NotEmpty();
        RuleFor(x => x.PriceId).NotEmpty();
        RuleFor(x => x.StartDateUtc).NotEmpty();

        RuleFor(x => x.EndDateUtc)
            .GreaterThan(x => x.StartDateUtc)
            .When(x => x.EndDateUtc.HasValue)
            .WithMessage("A data de término deve ser posterior à data de início.");
    }
}
