using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Features.Utenti.Commands.UpdateUtente;

public record UpdateUtenteCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public Guid? DefaultPriceId { get; init; }
    public bool IsActive { get; init; } = true;
}
