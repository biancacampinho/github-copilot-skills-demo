using MediatR;
using SkillGhcDemo.Application.Commands.Prices;
using SkillGhcDemo.Application.Common.Interfaces;
using SkillGhcDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SkillGhcDemo.Application.Handlers.Commands.Prices;

public class DeletePriceCommandHandler : IRequestHandler<DeletePriceCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<DeletePriceCommandHandler> _logger;

    public DeletePriceCommandHandler(IAppDbContext db, ILogger<DeletePriceCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Handle(DeletePriceCommand request, CancellationToken cancellationToken)
    {
        var price = await _db.Prices.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (price is null)
            return Result.NotFound($"Price {request.Id} not found.");

        // Prices are part of the history and are not referenced by FK from
        // OrderItem (which stores a snapshot of the value), so removal is safe.
        _db.Prices.Remove(price);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Price {PriceId} removed.", request.Id);
        return Result.Success();
    }
}
