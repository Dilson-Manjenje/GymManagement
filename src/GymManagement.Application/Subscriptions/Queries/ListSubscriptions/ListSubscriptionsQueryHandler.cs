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
        var subscriptions = await _subscriptionsRepository.ListAsync(cancellationToken);

        if (subscriptions is null || !subscriptions.Any())
            return new List<SubscriptionDto>();

        // TODO: Refactor: Create a ListWithDetails<SubscriptionDetailsDto> to return everything in one projected query
        // Instead of make multiple trips to repository
        var result = new List<SubscriptionDto>(subscriptions.Count());
        
        if(subscriptions is not null)
            foreach(var subs in subscriptions)
            {
                var rooms = await _subscriptionsRepository.ListSubscriptionRooms(subs.Id, cancellationToken);
                var roomNames = rooms.Select(r => r.Name).ToList()
                              ?? new List<string>();
                              
                result.Add(SubscriptionDto.MapToDto(subs, roomNames));
            }                
                                                        
        return result;                
    }
}