using MediatR;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;

namespace SkillGhcDemo.Application.Queries.Orders;

/// <summary>Lists orders that contain at least one item for the given product, with full user and product details.</summary>
public record GetOrdersByProductIdQuery(Guid ProductId) : IRequest<Result<IReadOnlyList<OrderWithDetailsDto>>>;
