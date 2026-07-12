using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Utenti.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Features.Utenti.Queries.GetUtenti;

public class GetUtentiQueryHandler : IRequestHandler<GetUtentiQuery, Result<IReadOnlyList<UtenteDto>>>
{
    private readonly IAppDbContext _db;

    public GetUtentiQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<UtenteDto>>> Handle(GetUtentiQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Utenti.AsNoTracking();

        if (request.OnlyActive)
            query = query.Where(u => u.IsActive);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(u => u.FullName.Contains(term) || u.Email.Contains(term));
        }

        var utenti = await query
            .OrderBy(u => u.FullName)
            .ToListAsync(cancellationToken);

        IReadOnlyList<UtenteDto> dtos = utenti.Select(u => u.ToDto()).ToList();
        return Result<IReadOnlyList<UtenteDto>>.Success(dtos);
    }
}
