using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Features.Categories.Commands.DeleteCategory;

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
            return Result.NotFound($"Category {request.Id} não encontrada.");

        if (category.Products.Any())
            return Result.Failure(
                "Não é possível excluir uma categoria com produtos associados.",
                ResultErrorType.Conflict);

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category {CategoryId} removida.", request.Id);
        return Result.Success();
    }
}
