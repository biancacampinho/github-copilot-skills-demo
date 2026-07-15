using MediatR;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Orders;
using Microsoft.EntityFrameworkCore;

namespace SkillGhcDemo.Application.Handlers.Queries.Orders;

public class GetOrdersByProductIdQueryHandler : IRequestHandler<GetOrdersByProductIdQuery, Result<IReadOnlyList<OrderByProductDto>>>
{
    private readonly IAppDbContext _db;

    public GetOrdersByProductIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<OrderByProductDto>>> Handle(GetOrdersByProductIdQuery request, CancellationToken cancellationToken)
    {
        var productExists = await _db.Products
            .AsNoTracking()
            .AnyAsync(p => p.Id == request.ProductId, cancellationToken);

        if (!productExists)
            return Result<IReadOnlyList<OrderByProductDto>>.NotFound($"Product {request.ProductId} not found.");

        var items = await _db.OrderItems
            .AsNoTracking()
            .Where(i => i.ProductId == request.ProductId)
            .Include(i => i.Order).ThenInclude(o => o.User)
            .Include(i => i.Product).ThenInclude(p => p.Prices)
            .OrderByDescending(i => i.Order.OrderDateUtc)
            .ToListAsync(cancellationToken);

        IReadOnlyList<OrderByProductDto> dtos = items.Select(i => i.ToOrderByProductDto()).ToList();
        return Result<IReadOnlyList<OrderByProductDto>>.Success(dtos);
    }
}
