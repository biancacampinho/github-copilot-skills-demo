using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Features.Utenti.Commands.DeleteUtente;

public class DeleteUtenteCommandHandler : IRequestHandler<DeleteUtenteCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<DeleteUtenteCommandHandler> _logger;

    public DeleteUtenteCommandHandler(IAppDbContext db, ILogger<DeleteUtenteCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteUtenteCommand request, CancellationToken cancellationToken)
    {
        var utente = await _db.Utenti.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (utente is null)
            return Result.NotFound($"Utente {request.Id} não encontrado.");

        _db.Utenti.Remove(utente);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Utente {UtenteId} removido.", request.Id);
        return Result.Success();
    }
}
