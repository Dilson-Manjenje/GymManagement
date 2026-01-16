using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.ListSubscriptions;

public class ListSubscriptionsQueryHandler : IRequestHandler<ListSubscriptionsQuery, ErrorOr<IEnumerable<SubscriptionDto>?>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public ListSubscriptionsQueryHandler(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }
    
    public async Task<ErrorOr<IEnumerable<SubscriptionDto>?>> Handle(ListSubscriptionsQuery query, CancellationToken cancellationToken = default)
    {
        var subscriptions = await _subscriptionsRepository.ListAsync();

        return subscriptions?.Select(susbcription => SubscriptionDto.MapToDto(susbcription)).ToList();                
    }
}