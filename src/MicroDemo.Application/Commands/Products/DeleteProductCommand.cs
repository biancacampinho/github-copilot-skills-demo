using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Commands.Products;

public record DeleteProductCommand(Guid Id) : IRequest<Result>;
