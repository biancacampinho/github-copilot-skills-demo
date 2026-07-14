using MediatR;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Prices;
using Microsoft.EntityFrameworkCore;

namespace SkillGhcDemo.Application.Handlers.Queries.Prices;

public class GetPricesQueryHandler : IRequestHandler<GetPricesQuery, Result<IReadOnlyList<PriceDto>>>
{
    private readonly IAppDbContext _db;

    public GetPricesQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<PriceDto>>> Handle(GetPricesQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Prices.AsNoTracking();

        if (request.ProductId is not null)
            query = query.Where(p => p.ProductId == request.ProductId);

        if (request.OnlyActive)
            query = query.Where(p => p.IsActive);

        var prices = await query
            .OrderByDescending(p => p.ValidFromUtc)
            .ToListAsync(cancellationToken);

        IReadOnlyList<PriceDto> dtos = prices.Select(p => p.ToDto()).ToList();
        return Result<IReadOnlyList<PriceDto>>.Success(dtos);
    }
}
