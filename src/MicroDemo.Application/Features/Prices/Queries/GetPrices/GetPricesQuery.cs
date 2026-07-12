using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Prices.Dtos;

namespace MicroDemo.Application.Features.Prices.Queries.GetPrices;

/// <summary>Lista preços, com filtros opcionais por produto e estado (ativo).</summary>
public record GetPricesQuery(Guid? ProductId = null, bool OnlyActive = false)
    : IRequest<Result<IReadOnlyList<PriceDto>>>;
