using MediatR;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Products;
using Microsoft.EntityFrameworkCore;

namespace SkillGhcDemo.Application.Handlers.Queries.Products;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IAppDbContext _db;

    public GetProductByIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .AsNoTracking()
            .Include(p => p.Prices)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        return product is null
            ? Result<ProductDto>.NotFound($"Product {request.Id} not found.")
            : Result<ProductDto>.Success(product.ToDto());
    }
}
