using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Products.Dtos;

namespace MicroDemo.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;
