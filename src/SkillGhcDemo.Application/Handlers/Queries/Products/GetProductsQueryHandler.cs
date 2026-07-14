using MediatR;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Products;
using Microsoft.EntityFrameworkCore;

namespace SkillGhcDemo.Application.Handlers.Queries.Products;

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
