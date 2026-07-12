using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(IAppDbContext db, ILogger<UpdateCategoryCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (category is null)
            return Result.NotFound($"Category {request.Id} não encontrada.");

        var nameTaken = await _db.Categories
            .AnyAsync(c => c.Name == request.Name && c.Id != request.Id, cancellationToken);
        if (nameTaken)
            return Result.Failure($"O nome '{request.Name}' já está em uso por outra categoria.", ResultErrorType.Conflict);

        category.Name = request.Name;
        category.Description = request.Description;
        category.IsActive = request.IsActive;
        category.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category {CategoryId} atualizada.", category.Id);
        return Result.Success();
    }
}
