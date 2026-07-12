using MediatR;
using MicroDemo.Application.Common.Models;

namespace MicroDemo.Application.Features.Prices.Commands.DeletePrice;

public record DeletePriceCommand(Guid Id) : IRequest<Result>;
