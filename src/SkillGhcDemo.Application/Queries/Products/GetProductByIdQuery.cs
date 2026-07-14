using MediatR;
using SkillGhcDemo.Application.Common.Models;
using SkillGhcDemo.Application.Dtos;

namespace SkillGhcDemo.Application.Queries.Products;

public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;
