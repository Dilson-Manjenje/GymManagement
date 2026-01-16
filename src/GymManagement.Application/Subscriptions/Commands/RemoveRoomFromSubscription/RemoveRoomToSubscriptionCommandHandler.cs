using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Commands.RemoveRoomFromSubscription;

public class RemoveRoomToSubscriptionCommandHandler : IRequestHandler<RemoveRoomToSubscriptionCommand, ErrorOr<Unit>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IRoomsRepository _roomsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RemoveRoomToSubscriptionCommandHandler(IUnitOfWork unitOfWork,
                                              ISubscriptionsRepository subscriptionsRepository,
                                              IRoomsRepository roomsRepository)
    {
        _unitOfWork = unitOfWork;
        _subscriptionsRepository = subscriptionsRepository;
        _roomsRepository = roomsRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(RemoveRoomToSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription is null)
            SubscriptionErrors.SubscriptionNotFound(command.SubscriptionId);

        var room = await _roomsRepository.GetByIdAsync(command.RoomId);
        if (room is null)
            RoomErrors.RoomNotFound(command.RoomId);

        if (!subscription!.HasRoom(command.RoomId))
            return SubscriptionErrors.RoomNotInSubscription(command.RoomId);

        var subsRoom = subscription.SubscriptionRooms.Single(sr => sr.RoomId == command.RoomId);
        
        await _subscriptionsRepository.RemoveRoomFromSubscriptionAsync(subsRoom);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}