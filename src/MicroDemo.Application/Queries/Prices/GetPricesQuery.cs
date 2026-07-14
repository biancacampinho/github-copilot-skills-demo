using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;

namespace MicroDemo.Application.Queries.Prices;

/// <summary>Lists prices, with optional filters by product and state (active).</summary>
public record GetPricesQuery(Guid? ProductId = null, bool OnlyActive = false)
    : IRequest<Result<IReadOnlyList<PriceDto>>>;
