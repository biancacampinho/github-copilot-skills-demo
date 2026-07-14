using MediatR;
using SkillGhcDemo.Application.Common.Models;

namespace SkillGhcDemo.Application.Commands.Categories;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
