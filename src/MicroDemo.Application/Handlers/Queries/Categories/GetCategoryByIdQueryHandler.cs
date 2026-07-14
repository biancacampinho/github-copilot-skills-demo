using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;
using MicroDemo.Application.Queries.Categories;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Handlers.Queries.Categories;

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
