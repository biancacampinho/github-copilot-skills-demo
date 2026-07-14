using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;

namespace MicroDemo.Application.Queries.Orders;

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;
