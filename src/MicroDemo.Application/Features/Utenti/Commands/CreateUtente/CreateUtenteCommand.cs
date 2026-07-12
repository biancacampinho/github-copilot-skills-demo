using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Features.Utenti.Commands.CreateUtente;

public record CreateUtenteCommand : IRequest<Result<Guid>>
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public Guid? DefaultPriceId { get; init; }
}
