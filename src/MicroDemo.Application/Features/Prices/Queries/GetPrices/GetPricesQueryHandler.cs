using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Prices.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Features.Prices.Queries.GetPrices;

public class GetPricesQueryHandler : IRequestHandler<GetPricesQuery, Result<IReadOnlyList<PriceDto>>>
{
    private readonly IAppDbContext _db;

    public GetPricesQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<PriceDto>>> Handle(GetPricesQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Prices.AsNoTracking();

        if (request.OnlyActive)
            query = query.Where(p => p.IsActive);

        var prices = await query
            .OrderBy(p => p.Amount)
            .ToListAsync(cancellationToken);

        IReadOnlyList<PriceDto> dtos = prices.Select(p => p.ToDto()).ToList();
        return Result<IReadOnlyList<PriceDto>>.Success(dtos);
    }
}
