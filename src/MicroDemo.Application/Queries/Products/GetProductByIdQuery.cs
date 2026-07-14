using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Dtos;

namespace MicroDemo.Application.Queries.Products;

public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;
