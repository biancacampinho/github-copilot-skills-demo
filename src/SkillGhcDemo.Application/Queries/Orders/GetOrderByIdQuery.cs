using MediatR;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;

namespace SkillGhcDemo.Application.Queries.Orders;

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;
