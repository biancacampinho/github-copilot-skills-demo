using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Products.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<IReadOnlyList<ProductDto>>>
{
    private readonly IAppDbContext _db;

    public GetProductsQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Products.AsNoTracking().Include(p => p.Prices).AsQueryable();

        if (request.OnlyActive)
            query = query.Where(p => p.IsActive);

        if (request.CategoryId is not null)
            query = query.Where(p => p.CategoryId == request.CategoryId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(p => p.Name.Contains(term) || p.Sku.Contains(term));
        }

        var products = await query
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        IReadOnlyList<ProductDto> dtos = products.Select(p => p.ToDto()).ToList();
        return Result<IReadOnlyList<ProductDto>>.Success(dtos);
    }
}
