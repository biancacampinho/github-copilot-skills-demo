using MediatR;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Categories;
using Microsoft.EntityFrameworkCore;

namespace SkillGhcDemo.Application.Handlers.Queries.Categories;

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
