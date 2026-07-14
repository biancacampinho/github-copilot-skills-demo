using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Commands.Categories;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
