using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Categories.Dtos;

namespace MicroDemo.Application.Features.Categories.Queries.GetCategories;

/// <summary>Lista categorias, opcionalmente filtrando apenas as ativas.</summary>
public record GetCategoriesQuery(bool OnlyActive = false) : IRequest<Result<IReadOnlyList<CategoryDto>>>;
