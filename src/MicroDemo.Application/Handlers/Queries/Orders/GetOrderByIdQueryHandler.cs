using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;
using MicroDemo.Application.Queries.Orders;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Handlers.Queries.Orders;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly IAppDbContext _db;

    public GetOrderByIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        return order is null
            ? Result<OrderDto>.NotFound($"Order {request.Id} not found.")
            : Result<OrderDto>.Success(order.ToDto());
    }
}
