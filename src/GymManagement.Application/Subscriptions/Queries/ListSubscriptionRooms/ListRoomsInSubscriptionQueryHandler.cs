using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Rooms.Queries.Dtos;
using GymManagement.Application.Subscriptions.Queries.Dtos;
using GymManagement.Application.Subscriptions.Queries.ListSubscriptionRooms;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.ListSubscriptionsRooms;

public class ListSubscriptionRoomsQueryHandler : IRequestHandler<ListRoomsInSubscriptionQuery, ErrorOr<IEnumerable<RoomDto>?>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;

    public ListSubscriptionRoomsQueryHandler(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }

    public async Task<ErrorOr<IEnumerable<RoomDto>?>> Handle(ListRoomsInSubscriptionQuery query, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(query.SubscriptionId);
        if (subscription is null)
            return SubscriptionErrors.SubscriptionNotFound(query.SubscriptionId);

        if (!subscription.IsActive)
            return SubscriptionErrors.ExpiredSubscription(query.SubscriptionId);

        var rooms = await _subscriptionsRepository.ListSubscriptionRooms(query.SubscriptionId);

        if (rooms is null || !rooms.Any())
            return new List<RoomDto>();

        return rooms?.Select(room => RoomDto.MapToDto(room)).ToList();       
    }
}