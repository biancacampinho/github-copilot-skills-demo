using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Utenti.Dtos;

namespace MicroDemo.Application.Features.Utenti.Queries.GetUtenteById;

public record GetUtenteByIdQuery(Guid Id) : IRequest<Result<UtenteDto>>;
