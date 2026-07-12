using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Categories.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Features.Categories.Queries.GetCategoryById;

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
            ? Result<CategoryDto>.NotFound($"Category {request.Id} não encontrada.")
            : Result<CategoryDto>.Success(category.ToDto());
    }
}
