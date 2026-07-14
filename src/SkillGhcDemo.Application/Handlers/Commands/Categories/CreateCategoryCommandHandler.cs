using MediatR;
using SkillGhcDemo.Application.Commands.Categories;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SkillGhcDemo.Application.Handlers.Commands.Categories;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(IAppDbContext db, ILogger<CreateCategoryCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var nameExists = await _db.Categories
            .AnyAsync(c => c.Name == request.Name, cancellationToken);
        if (nameExists)
            return Result<Guid>.Failure($"A category with the name '{request.Name}' already exists.", ResultErrorType.Conflict);

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category {CategoryId} created.", category.Id);
        return Result<Guid>.Success(category.Id);
    }
}
