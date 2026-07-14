using MediatR;
using SkillGhcDemo.Application.Commands.Categories;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SkillGhcDemo.Application.Handlers.Commands.Categories;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;

    public DeleteCategoryCommandHandler(IAppDbContext db, ILogger<DeleteCategoryCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _db.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null)
            return Result.NotFound($"Category {request.Id} not found.");

        if (category.Products.Any())
            return Result.Failure(
                "Cannot delete a category that has associated products.",
                ResultErrorType.Conflict);

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category {CategoryId} removed.", request.Id);
        return Result.Success();
    }
}
