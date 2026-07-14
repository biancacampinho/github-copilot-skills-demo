using MediatR;
using SkillGhcDemo.Application.Commands.Products;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SkillGhcDemo.Application.Handlers.Commands.Products;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(IAppDbContext db, ILogger<UpdateProductCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product is null)
            return Result.NotFound($"Product {request.Id} not found.");

        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
            return Result.NotFound($"Category {request.CategoryId} not found.");

        var skuTaken = await _db.Products
            .AnyAsync(p => p.Sku == request.Sku && p.Id != request.Id, cancellationToken);
        if (skuTaken)
            return Result.Failure($"The SKU '{request.Sku}' is already in use by another product.", ResultErrorType.Conflict);

        product.Name = request.Name;
        product.Description = request.Description;
        product.Sku = request.Sku;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} updated.", product.Id);
        return Result.Success();
    }
}
