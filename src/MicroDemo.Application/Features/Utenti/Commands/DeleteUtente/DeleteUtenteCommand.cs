using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Features.Utenti.Commands.DeleteUtente;

public record DeleteUtenteCommand(Guid Id) : IRequest<Result>;
