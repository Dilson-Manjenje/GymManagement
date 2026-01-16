using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Commands.AddRoomToSubscription;

public class AddRoomToSubscriptionCommandHandler : IRequestHandler<AddRoomToSubscriptionCommand, ErrorOr<Guid>>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IRoomsRepository _roomsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public AddRoomToSubscriptionCommandHandler(IUnitOfWork unitOfWork,
                                              ISubscriptionsRepository subscriptionsRepository,
                                              IRoomsRepository roomsRepository)
    {
        _unitOfWork = unitOfWork;
        _subscriptionsRepository = subscriptionsRepository;
        _roomsRepository = roomsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(AddRoomToSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription is null)
            return SubscriptionErrors.SubscriptionNotFound(command.SubscriptionId);

        var room = await _roomsRepository.GetByIdAsync(command.RoomId);
        if (room is null)
            return RoomErrors.RoomNotFound(command.RoomId);

        if (room.GymId != subscription.Member.GymId)
            return SubscriptionErrors.RoomWasNotFoundInMemberGym(roomId: room.Id);

        if (subscription.HasRoom(command.RoomId))
            return SubscriptionErrors.RoomAlreadyAssociated(command.RoomId);

        if (subscription.NumberOfRooms >= subscription.MaxRoomsAllowed)
            return SubscriptionErrors.HasMaxRoomsAllowed();

        var subRoom = new SubscriptionRooms(subscription.Id, room.Id);

        await _subscriptionsRepository.AddRoomToSubscriptionAsync(subRoom);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return subscription.Id;
    }
}