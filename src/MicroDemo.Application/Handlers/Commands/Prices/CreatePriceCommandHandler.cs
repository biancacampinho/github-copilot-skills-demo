using MediatR;
using MicroDemo.Application.Commands.Prices;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Handlers.Commands.Prices;

public class CreatePriceCommandHandler : IRequestHandler<CreatePriceCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<CreatePriceCommandHandler> _logger;

    public CreatePriceCommandHandler(IAppDbContext db, ILogger<CreatePriceCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreatePriceCommand request, CancellationToken cancellationToken)
    {
        var productExists = await _db.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken);
        if (!productExists)
            return Result<Guid>.NotFound($"Product {request.ProductId} not found.");

        var price = new Price
        {
            ProductId = request.ProductId,
            Amount = request.Amount,
            Currency = request.Currency.ToUpperInvariant(),
            ValidFromUtc = request.ValidFromUtc ?? DateTime.UtcNow,
            IsActive = request.IsActive
        };

        _db.Prices.Add(price);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Price {PriceId} created for product {ProductId}.", price.Id, price.ProductId);
        return Result<Guid>.Success(price.Id);
    }
}
