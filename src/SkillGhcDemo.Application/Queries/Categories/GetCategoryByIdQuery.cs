using MediatR;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;

namespace SkillGhcDemo.Application.Queries.Categories;

public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDto>>;
