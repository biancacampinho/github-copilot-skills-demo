using MediatR;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;

namespace SkillGhcDemo.Application.Queries.Prices;

public record GetPriceByIdQuery(Guid Id) : IRequest<Result<PriceDto>>;
