using MediatR;
using MicroDemo.Application.Commands.Products;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Handlers.Commands.Products;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IAppDbContext db, ILogger<CreateProductCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
            return Result<Guid>.NotFound($"Category {request.CategoryId} not found.");

        var skuExists = await _db.Products.AnyAsync(p => p.Sku == request.Sku, cancellationToken);
        if (skuExists)
            return Result<Guid>.Failure($"A product with the SKU '{request.Sku}' already exists.", ResultErrorType.Conflict);

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Sku = request.Sku,
            CategoryId = request.CategoryId,
            IsActive = request.IsActive
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} created.", product.Id);
        return Result<Guid>.Success(product.Id);
    }
}
