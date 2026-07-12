using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Features.Subscriptions.Commands.CreateSubscription;

// ═══════════════════════════════════════════════════════════════════════════════
//  ★★★  ENDPOINT RESERVADO PARA IMPLEMENTAÇÃO MANUAL  ★★★
//
//  Este handler foi DELIBERADAMENTE deixado como stub para ser implementado
//  manualmente com a sua skill personalizada.
//
//  POR QUE ESTE ENDPOINT FOI ESCOLHIDO:
//   • É o único caso de uso com LÓGICA DE NEGÓCIO CRUZADA entre agregados
//     (Utente + Price), diferente do CRUD trivial dos demais endpoints.
//   • Exercita validações que dependem do banco (não cabem no FluentValidator),
//     regras de estado (assinatura ativa sobreposta) e mapeamento de erros para
//     o envelope Result<T>. Alto valor didático.
//
//  CHECKLIST DO QUE IMPLEMENTAR (regras de negócio sugeridas):
//   1. Verificar se o Utente (request.UtenteId) existe    → senão Result.NotFound
//   2. Verificar se o Price (request.PriceId) existe        → senão Result.NotFound
//   3. Verificar se o Price está ativo (IsActive == true)   → senão Result.Failure/Conflict
//   4. Impedir assinatura ATIVA sobreposta para o mesmo Utente
//      (mesma janela de datas)                              → senão Result.Conflict
//   5. Criar a entidade Subscription (Status = Active), persistir e
//      retornar Result<Guid>.Success(subscription.Id)
//   6. Logar a criação com _logger.LogInformation(...)
//
//  Dependências já injetadas e prontas para uso: _db (IAppDbContext) e _logger.
//  O Command, o Validator e o endpoint no SubscriptionsController já existem.
// ═══════════════════════════════════════════════════════════════════════════════
public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<CreateSubscriptionCommandHandler> _logger;

    public CreateSubscriptionCommandHandler(IAppDbContext db, ILogger<CreateSubscriptionCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task<Result<Guid>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        // TODO(skill): implementar as regras descritas no cabeçalho deste arquivo.
        throw new NotImplementedException(
            "CreateSubscriptionCommandHandler está reservado para implementação manual via skill personalizada.");
    }
}
