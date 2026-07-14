using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Commands.Prices;

public record DeletePriceCommand(Guid Id) : IRequest<Result>;
