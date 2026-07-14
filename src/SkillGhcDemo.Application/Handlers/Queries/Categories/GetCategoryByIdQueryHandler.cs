using MediatR;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;
using SkillGhcDemo.Application.Queries.Categories;
using Microsoft.EntityFrameworkCore;

namespace SkillGhcDemo.Application.Handlers.Queries.Categories;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly IAppDbContext _db;

    public GetCategoryByIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        return category is null
            ? Result<CategoryDto>.NotFound($"Category {request.Id} not found.")
            : Result<CategoryDto>.Success(category.ToDto());
    }
}
