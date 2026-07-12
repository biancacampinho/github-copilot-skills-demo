using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Categories.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
{
    private readonly IAppDbContext _db;

    public GetCategoriesQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Categories.AsNoTracking();

        if (request.OnlyActive)
            query = query.Where(c => c.IsActive);

        var categories = await query
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        IReadOnlyList<CategoryDto> dtos = categories.Select(c => c.ToDto()).ToList();
        return Result<IReadOnlyList<CategoryDto>>.Success(dtos);
    }
}
