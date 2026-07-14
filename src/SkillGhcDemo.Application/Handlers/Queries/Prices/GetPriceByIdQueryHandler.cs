using MediatR;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Prices;
using Microsoft.EntityFrameworkCore;

namespace SkillGhcDemo.Application.Handlers.Queries.Prices;

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
