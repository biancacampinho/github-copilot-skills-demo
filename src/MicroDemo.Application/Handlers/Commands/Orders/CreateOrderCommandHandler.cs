using MediatR;
using MicroDemo.Application.Commands.Orders;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Handlers.Commands.Orders;

// ═══════════════════════════════════════════════════════════════════════════════
//  ★★★  ENDPOINT RESERVED FOR MANUAL IMPLEMENTATION  ★★★
//
//  This handler was DELIBERATELY left as a stub to be implemented
//  manually with your custom skill.
//
//  WHY THIS ENDPOINT WAS CHOSEN:
//   • It is the use case RICHEST IN BUSINESS RULES and the only one that spans several
//     aggregates (User + Product + Price + Order + OrderItem), unlike the
//     trivial CRUD of the other endpoints.
//   • It exercises validations that depend on the database (they do not fit in the FluentValidator),
//     resolution of the current price + SNAPSHOT in the OrderItem, calculation of totals and
//     mapping of errors to the Result<T> envelope. High educational value.
//
//  CHECKLIST OF WHAT TO IMPLEMENT (suggested business rules):
//   1. Check whether the User (request.UserId) exists         → otherwise Result.NotFound
//   2. For each item:
//        a. Load the Product and confirm that it exists        → otherwise Result.NotFound
//        b. Confirm that the Product is active                 → otherwise Result.Failure/Conflict
//        c. Resolve the CURRENT PRICE (active Price with the
//           most recent ValidFromUtc of the product)           → otherwise Result.Conflict
//        d. Create the OrderItem with UnitPrice = price snapshot,
//           Currency from the price, Quantity from the request and
//           LineTotal = UnitPrice × Quantity
//   3. (Optional) Ensure a single currency across the whole order → otherwise Result.Conflict
//   4. Calculate Order.TotalAmount = sum of the LineTotal, Status = Pending
//   5. Persist Order + OrderItems and return Result<Guid>.Success(order.Id)
//   6. Log the creation with _logger.LogInformation(...)
//
//  Dependencies already injected and ready to use: _db (IAppDbContext) and _logger.
//  The Command, the Validator and the endpoint in the OrdersController already exist.
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
        // TODO(skill): implement the rules described in the header of this file.
        throw new NotImplementedException(
            "CreateOrderCommandHandler is reserved for manual implementation via custom skill.");
    }
}
