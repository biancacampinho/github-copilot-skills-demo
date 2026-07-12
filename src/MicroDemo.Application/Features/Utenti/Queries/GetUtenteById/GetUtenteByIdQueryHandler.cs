using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Utenti.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Features.Utenti.Queries.GetUtenteById;

public class GetUtenteByIdQueryHandler : IRequestHandler<GetUtenteByIdQuery, Result<UtenteDto>>
{
    private readonly IAppDbContext _db;

    public GetUtenteByIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<UtenteDto>> Handle(GetUtenteByIdQuery request, CancellationToken cancellationToken)
    {
        var utente = await _db.Utenti
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        return utente is null
            ? Result<UtenteDto>.NotFound($"Utente {request.Id} não encontrado.")
            : Result<UtenteDto>.Success(utente.ToDto());
    }
}
