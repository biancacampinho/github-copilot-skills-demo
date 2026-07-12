using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Prices.Dtos;

namespace MicroDemo.Application.Features.Prices.Queries.GetPriceById;

public record GetPriceByIdQuery(Guid Id) : IRequest<Result<PriceDto>>;
