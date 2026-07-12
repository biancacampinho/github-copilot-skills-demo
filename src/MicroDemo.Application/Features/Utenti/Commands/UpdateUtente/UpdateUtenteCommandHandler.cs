using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Features.Utenti.Commands.UpdateUtente;

public class UpdateUtenteCommandHandler : IRequestHandler<UpdateUtenteCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<UpdateUtenteCommandHandler> _logger;

    public UpdateUtenteCommandHandler(IAppDbContext db, ILogger<UpdateUtenteCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateUtenteCommand request, CancellationToken cancellationToken)
    {
        var utente = await _db.Utenti.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (utente is null)
            return Result.NotFound($"Utente {request.Id} não encontrado.");

        var emailTaken = await _db.Utenti
            .AnyAsync(u => u.Email == request.Email && u.Id != request.Id, cancellationToken);
        if (emailTaken)
            return Result.Failure($"O e-mail '{request.Email}' já está em uso por outro utente.", ResultErrorType.Conflict);

        if (request.DefaultPriceId is not null)
        {
            var priceExists = await _db.Prices.AnyAsync(p => p.Id == request.DefaultPriceId, cancellationToken);
            if (!priceExists)
                return Result.NotFound($"Price {request.DefaultPriceId} não encontrado.");
        }

        utente.FullName = request.FullName;
        utente.Email = request.Email;
        utente.PhoneNumber = request.PhoneNumber;
        utente.DefaultPriceId = request.DefaultPriceId;
        utente.IsActive = request.IsActive;
        utente.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Utente {UtenteId} atualizado.", utente.Id);
        return Result.Success();
    }
}
