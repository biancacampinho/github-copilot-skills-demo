using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest<Result>;
