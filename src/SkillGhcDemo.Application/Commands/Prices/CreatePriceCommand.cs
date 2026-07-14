using MediatR;
using SkillGhcDemo.Application.Common.Models;

namespace SkillGhcDemo.Application.Commands.Prices;

public record CreatePriceCommand : IRequest<Result<Guid>>
{
    public Guid ProductId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "EUR";
    public DateTime? ValidFromUtc { get; init; }
    public bool IsActive { get; init; } = true;
}
