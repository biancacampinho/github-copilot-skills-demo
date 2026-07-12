using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Prices.Dtos;

namespace MicroDemo.Application.Features.Prices.Queries.GetPrices;

/// <summary>Lista preços, opcionalmente filtrando apenas os ativos.</summary>
public record GetPricesQuery(bool OnlyActive = false) : IRequest<Result<IReadOnlyList<PriceDto>>>;
