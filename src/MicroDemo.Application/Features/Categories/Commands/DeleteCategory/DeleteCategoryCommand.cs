using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
