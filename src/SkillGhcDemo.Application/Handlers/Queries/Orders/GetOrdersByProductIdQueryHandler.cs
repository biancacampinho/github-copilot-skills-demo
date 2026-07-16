using MediatR;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Orders;
using Microsoft.EntityFrameworkCore;

namespace SkillGhcDemo.Application.Handlers.Queries.Orders;

public class GetOrdersByProductIdQueryHandler : IRequestHandler<GetOrdersByProductIdQuery, Result<IReadOnlyList<OrderWithDetailsDto>>>
{
    private readonly IAppDbContext _db;

    public GetOrdersByProductIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<OrderWithDetailsDto>>> Handle(GetOrdersByProductIdQuery request, CancellationToken cancellationToken)
    {
        var orders = await _db.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Prices)
            .Where(o => o.Items.Any(i => i.ProductId == request.ProductId))
            .OrderBy(o => o.OrderDateUtc)
            .ToListAsync(cancellationToken);

        IReadOnlyList<OrderWithDetailsDto> dtos = orders.Select(o => o.ToDetailsDto()).ToList();
        return Result<IReadOnlyList<OrderWithDetailsDto>>.Success(dtos);
    }
}
