using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Features.Orders.Commands.CreateOrder;

// ═══════════════════════════════════════════════════════════════════════════════
//  ★★★  ENDPOINT RESERVADO PARA IMPLEMENTAÇÃO MANUAL  ★★★
//
//  Este handler foi DELIBERADAMENTE deixado como stub para ser implementado
//  manualmente com a sua skill personalizada.
//
//  POR QUE ESTE ENDPOINT FOI ESCOLHIDO:
//   • É o caso de uso mais RICO EM REGRAS DE NEGÓCIO e o único que cruza vários
//     agregados (User + Product + Price + Order + OrderItem), ao contrário do
//     CRUD trivial dos demais endpoints.
//   • Exercita validações que dependem do banco (não cabem no FluentValidator),
//     resolução do preço corrente + SNAPSHOT no OrderItem, cálculo de totais e
//     mapeamento de erros para o envelope Result<T>. Alto valor didático.
//
//  CHECKLIST DO QUE IMPLEMENTAR (regras de negócio sugeridas):
//   1. Verificar se o User (request.UserId) existe          → senão Result.NotFound
//   2. Para cada item:
//        a. Carregar o Product e confirmar que existe        → senão Result.NotFound
//        b. Confirmar que o Product está ativo               → senão Result.Failure/Conflict
//        c. Resolver o PREÇO CORRENTE (Price ativo com o
//           ValidFromUtc mais recente do produto)            → senão Result.Conflict
//        d. Criar o OrderItem com UnitPrice = snapshot do preço,
//           Currency do preço, Quantity do request e
//           LineTotal = UnitPrice × Quantity
//   3. (Opcional) Garantir moeda única em todo o pedido      → senão Result.Conflict
//   4. Calcular Order.TotalAmount = soma dos LineTotal, Status = Pending
//   5. Persistir Order + OrderItems e devolver Result<Guid>.Success(order.Id)
//   6. Logar a criação com _logger.LogInformation(...)
//
//  Dependências já injetadas e prontas para uso: _db (IAppDbContext) e _logger.
//  O Command, o Validator e o endpoint no OrdersController já existem.
// ═══════════════════════════════════════════════════════════════════════════════
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(IAppDbContext db, ILogger<CreateOrderCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // TODO(skill): implementar as regras descritas no cabeçalho deste arquivo.
        throw new NotImplementedException(
            "CreateOrderCommandHandler está reservado para implementação manual via skill personalizada.");
    }
}
