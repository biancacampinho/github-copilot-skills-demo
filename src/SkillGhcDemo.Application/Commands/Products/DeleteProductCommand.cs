using MediatR;
using SkillGhcDemo.Application.Common.Models;

namespace SkillGhcDemo.Application.Commands.Products;

public record DeleteProductCommand(Guid Id) : IRequest<Result>;
