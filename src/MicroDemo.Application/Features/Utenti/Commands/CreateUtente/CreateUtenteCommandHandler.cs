using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicroDemo.Application.Features.Utenti.Commands.CreateUtente;

public class CreateUtenteCommandHandler : IRequestHandler<CreateUtenteCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<CreateUtenteCommandHandler> _logger;

    public CreateUtenteCommandHandler(IAppDbContext db, ILogger<CreateUtenteCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateUtenteCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _db.Utenti
            .AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (emailExists)
            return Result<Guid>.Failure($"Já existe um utente com o e-mail '{request.Email}'.", ResultErrorType.Conflict);

        if (request.DefaultPriceId is not null)
        {
            var priceExists = await _db.Prices.AnyAsync(p => p.Id == request.DefaultPriceId, cancellationToken);
            if (!priceExists)
                return Result<Guid>.NotFound($"Price {request.DefaultPriceId} não encontrado.");
        }

        var utente = new Utente
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DefaultPriceId = request.DefaultPriceId
        };

        _db.Utenti.Add(utente);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Utente {UtenteId} criado.", utente.Id);
        return Result<Guid>.Success(utente.Id);
    }
}
