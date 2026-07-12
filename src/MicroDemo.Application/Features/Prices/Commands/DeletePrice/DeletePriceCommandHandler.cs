using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Features.Prices.Commands.DeletePrice;

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
            return Result.NotFound($"Price {request.Id} não encontrado.");

        // Preços fazem parte do histórico e não são referenciados por FK a partir de
        // OrderItem (que guarda um snapshot do valor), pelo que a remoção é segura.
        _db.Prices.Remove(price);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Price {PriceId} removido.", request.Id);
        return Result.Success();
    }
}
