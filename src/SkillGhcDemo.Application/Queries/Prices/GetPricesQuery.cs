using MediatR;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;

namespace SkillGhcDemo.Application.Queries.Prices;

/// <summary>Lists prices, with optional filters by product and state (active).</summary>
public record GetPricesQuery(Guid? ProductId = null, bool OnlyActive = false)
    : IRequest<Result<IReadOnlyList<PriceDto>>>;
