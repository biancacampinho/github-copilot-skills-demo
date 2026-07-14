using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;

namespace MicroDemo.Application.Queries.Categories;

/// <summary>Lists categories, optionally filtering only the active ones.</summary>
public record GetCategoriesQuery(bool OnlyActive = false) : IRequest<Result<IReadOnlyList<CategoryDto>>>;
