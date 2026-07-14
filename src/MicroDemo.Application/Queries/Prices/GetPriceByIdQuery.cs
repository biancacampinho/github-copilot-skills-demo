using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;

namespace MicroDemo.Application.Queries.Prices;

public record GetPriceByIdQuery(Guid Id) : IRequest<Result<PriceDto>>;
