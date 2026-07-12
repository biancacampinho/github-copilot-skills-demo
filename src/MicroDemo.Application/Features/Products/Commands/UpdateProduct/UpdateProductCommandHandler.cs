using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Features.Products.Commands.UpdateProduct;

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
            return Result.NotFound($"Product {request.Id} não encontrado.");

        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
            return Result.NotFound($"Category {request.CategoryId} não encontrada.");

        var skuTaken = await _db.Products
            .AnyAsync(p => p.Sku == request.Sku && p.Id != request.Id, cancellationToken);
        if (skuTaken)
            return Result.Failure($"O SKU '{request.Sku}' já está em uso por outro produto.", ResultErrorType.Conflict);

        product.Name = request.Name;
        product.Description = request.Description;
        product.Sku = request.Sku;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} atualizado.", product.Id);
        return Result.Success();
    }
}
