using MediatR;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;

namespace SkillGhcDemo.Application.Queries.Orders;

/// <summary>
/// Lists the order lines for a given product, one entry per <see cref="Domain.Entities.OrderItem"/>,
/// including the parent order's user and the product's own data.
/// </summary>
public record GetOrdersByProductIdQuery(Guid ProductId) : IRequest<Result<IReadOnlyList<OrderByProductDto>>>;
