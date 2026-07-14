using MediatR;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;

namespace SkillGhcDemo.Application.Queries.Products;

/// <summary>Lists products, with optional filters by text, category and state.</summary>
public record GetProductsQuery(string? Search = null, Guid? CategoryId = null, bool OnlyActive = false)
    : IRequest<Result<IReadOnlyList<ProductDto>>>;
