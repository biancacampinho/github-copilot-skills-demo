using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;
using MicroDemo.Application.Queries.Products;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Handlers.Queries.Products;

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
