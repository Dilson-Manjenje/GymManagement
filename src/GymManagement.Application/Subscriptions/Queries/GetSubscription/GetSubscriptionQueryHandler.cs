using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.GetSubscription;

public class GetSubscriptionQueryHandler : IRequestHandler<GetSubscriptionQuery, ErrorOr<SubscriptionDto>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public GetSubscriptionQueryHandler(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }
    
    public async Task<ErrorOr<SubscriptionDto>> Handle(GetSubscriptionQuery query, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(query.Id);
                            
        return (subscription is null) 
            ? SubscriptionErrors.SubscriptionNotFound(query.Id)
            : SubscriptionDto.MapToDto(subscription);        
    }
}