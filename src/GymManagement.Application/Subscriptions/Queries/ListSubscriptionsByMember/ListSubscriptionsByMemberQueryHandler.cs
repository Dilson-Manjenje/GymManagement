using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.ListSubscriptionsByMember;

public class ListSubscriptionsByMemberQueryHandler : IRequestHandler<ListSubscriptionsByMemberQuery, ErrorOr<IEnumerable<SubscriptionDto>?>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public ListSubscriptionsByMemberQueryHandler(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }

    public async Task<ErrorOr<IEnumerable<SubscriptionDto>?>> Handle(ListSubscriptionsByMemberQuery query, CancellationToken cancellationToken)
    {
          var subscriptions = await _subscriptionsRepository.ListByMemberAsync(query.MemberId);

        return subscriptions?.Select(susbcription => SubscriptionDto.MapToDto(susbcription)).ToList();       
    }
}