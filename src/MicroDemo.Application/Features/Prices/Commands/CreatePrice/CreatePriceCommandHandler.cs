using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Features.Prices.Commands.CreatePrice;

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
        var price = new Price
        {
            Name = request.Name,
            Description = request.Description,
            Amount = request.Amount,
            Currency = request.Currency.ToUpperInvariant(),
            BillingPeriod = request.BillingPeriod,
            IsActive = request.IsActive
        };

        _db.Prices.Add(price);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Price {PriceId} criado.", price.Id);
        return Result<Guid>.Success(price.Id);
    }
}
