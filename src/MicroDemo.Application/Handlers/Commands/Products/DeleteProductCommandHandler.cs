using MediatR;
using MicroDemo.Application.Commands.Products;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Handlers.Commands.Products;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(IAppDbContext db, ILogger<DeleteProductCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _db.Products
            .Include(p => p.OrderItems)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null)
            return Result.NotFound($"Product {request.Id} not found.");

        if (product.OrderItems.Any())
            return Result.Failure(
                "A product that appears in orders cannot be deleted.",
                ResultErrorType.Conflict);

        _db.Products.Remove(product);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} removed.", request.Id);
        return Result.Success();
    }
}
