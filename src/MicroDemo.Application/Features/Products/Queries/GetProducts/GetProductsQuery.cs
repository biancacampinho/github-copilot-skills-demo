using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Products.Dtos;

namespace MicroDemo.Application.Features.Products.Queries.GetProducts;

/// <summary>Lista produtos, com filtros opcionais de texto, categoria e estado.</summary>
public record GetProductsQuery(string? Search = null, Guid? CategoryId = null, bool OnlyActive = false)
    : IRequest<Result<IReadOnlyList<ProductDto>>>;
