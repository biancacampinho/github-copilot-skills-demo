using MediatR;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Subscriptions.Dtos;

namespace MicroDemo.Application.Features.Subscriptions.Queries.GetSubscriptionById;

public record GetSubscriptionByIdQuery(Guid Id) : IRequest<Result<SubscriptionDto>>;
