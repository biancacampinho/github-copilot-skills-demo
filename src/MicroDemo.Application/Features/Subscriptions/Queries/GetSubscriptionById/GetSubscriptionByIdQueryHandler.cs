using MediatR;
using MicroDemo.Application.Common.Interfaces;
using MicroDemo.Application.Common.Models;
using MicroDemo.Application.Features.Subscriptions.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MicroDemo.Application.Features.Subscriptions.Queries.GetSubscriptionById;

public class GetSubscriptionByIdQueryHandler : IRequestHandler<GetSubscriptionByIdQuery, Result<SubscriptionDto>>
{
    private readonly IAppDbContext _db;

    public GetSubscriptionByIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<SubscriptionDto>> Handle(GetSubscriptionByIdQuery request, CancellationToken cancellationToken)
    {
        var subscription = await _db.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        return subscription is null
            ? Result<SubscriptionDto>.NotFound($"Subscription {request.Id} não encontrada.")
            : Result<SubscriptionDto>.Success(subscription.ToDto());
    }
}
