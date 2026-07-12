using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Domain.Enums;

namespace MicroDemo.Application.Features.Prices.Commands.UpdatePrice;

public record UpdatePriceCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "EUR";
    public BillingPeriod BillingPeriod { get; init; } = BillingPeriod.Monthly;
    public bool IsActive { get; init; } = true;
}
