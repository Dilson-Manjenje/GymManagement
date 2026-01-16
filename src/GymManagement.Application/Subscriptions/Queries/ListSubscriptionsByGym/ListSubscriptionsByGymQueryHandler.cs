using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.ListSubscriptionsByGym;

public class ListSubscriptionsByGymQueryHandler : IRequestHandler<ListSubscriptionsByGymQuery, ErrorOr<IEnumerable<SubscriptionDto>?>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public ListSubscriptionsByGymQueryHandler(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }

    public async Task<ErrorOr<IEnumerable<SubscriptionDto>?>> Handle(ListSubscriptionsByGymQuery query, CancellationToken cancellationToken)
    {
          var subscriptions = await _subscriptionsRepository.ListByGymAsync(query.GymId);

        return subscriptions?.Select(susbcription => SubscriptionDto.MapToDto(susbcription)).ToList();       
    }
}