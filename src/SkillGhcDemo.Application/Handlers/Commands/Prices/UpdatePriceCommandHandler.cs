using MediatR;
using SkillGhcDemo.Application.Commands.Prices;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SkillGhcDemo.Application.Handlers.Commands.Prices;

public class UpdatePriceCommandHandler : IRequestHandler<UpdatePriceCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<UpdatePriceCommandHandler> _logger;

    public UpdatePriceCommandHandler(IAppDbContext db, ILogger<UpdatePriceCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdatePriceCommand request, CancellationToken cancellationToken)
    {
        var price = await _db.Prices.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (price is null)
            return Result.NotFound($"Price {request.Id} not found.");

        price.Amount = request.Amount;
        price.Currency = request.Currency.ToUpperInvariant();
        price.ValidFromUtc = request.ValidFromUtc;
        price.IsActive = request.IsActive;
        price.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Price {PriceId} updated.", price.Id);
        return Result.Success();
    }
}
