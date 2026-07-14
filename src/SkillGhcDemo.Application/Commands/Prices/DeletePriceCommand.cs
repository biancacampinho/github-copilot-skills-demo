using MediatR;
using SkillGhcDemo.Application.Common.Models;

namespace SkillGhcDemo.Application.Commands.Prices;

public record DeletePriceCommand(Guid Id) : IRequest<Result>;
