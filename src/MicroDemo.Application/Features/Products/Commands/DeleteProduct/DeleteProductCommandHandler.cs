using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Features.Products.Commands.DeleteProduct;

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
            return Result.NotFound($"Product {request.Id} não encontrado.");

        if (product.OrderItems.Any())
            return Result.Failure(
                "Não é possível excluir um produto presente em pedidos.",
                ResultErrorType.Conflict);

        _db.Products.Remove(product);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {ProductId} removido.", request.Id);
        return Result.Success();
    }
}
