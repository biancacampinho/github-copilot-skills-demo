using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;
using MicroDemo.Application.Queries.Prices;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Handlers.Queries.Prices;

public class GetPriceByIdQueryHandler : IRequestHandler<GetPriceByIdQuery, Result<PriceDto>>
{
    private readonly IAppDbContext _db;

    public GetPriceByIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<PriceDto>> Handle(GetPriceByIdQuery request, CancellationToken cancellationToken)
    {
        var price = await _db.Prices
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        return price is null
            ? Result<PriceDto>.NotFound($"Price {request.Id} not found.")
            : Result<PriceDto>.Success(price.ToDto());
    }
}
