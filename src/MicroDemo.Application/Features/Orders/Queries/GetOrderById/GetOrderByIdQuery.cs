using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Orders.Dtos;

namespace MicroDemo.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;
