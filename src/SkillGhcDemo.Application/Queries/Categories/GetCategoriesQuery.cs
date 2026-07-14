using MediatR;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;

namespace SkillGhcDemo.Application.Queries.Categories;

/// <summary>Lists categories, optionally filtering only the active ones.</summary>
public record GetCategoriesQuery(bool OnlyActive = false) : IRequest<Result<IReadOnlyList<CategoryDto>>>;
