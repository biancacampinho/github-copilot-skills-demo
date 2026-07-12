using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Categories.Dtos;

namespace MicroDemo.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDto>>;
