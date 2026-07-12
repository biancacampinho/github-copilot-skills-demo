using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Prices.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Features.Prices.Queries.GetPriceById;

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
            ? Result<PriceDto>.NotFound($"Price {request.Id} não encontrado.")
            : Result<PriceDto>.Success(price.ToDto());
    }
}
