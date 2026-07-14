using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;

namespace MicroDemo.Application.Queries.Categories;

public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDto>>;
