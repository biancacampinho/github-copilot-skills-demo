using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Commands.Prices;

public record UpdatePriceCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "EUR";
    public DateTime ValidFromUtc { get; init; }
    public bool IsActive { get; init; } = true;
}
