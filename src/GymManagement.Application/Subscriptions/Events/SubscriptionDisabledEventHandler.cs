using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Subscriptions;
using GymManagement.Domain.Subscriptions.Events;
using MediatR;
using OneOf.Types;

namespace GymManagement.Application.Subscriptions.Events;

public class SubscriptionDisabledEventHandler : INotificationHandler<SubscriptionDisabledEvent>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionDisabledEventHandler(ISubscriptionsRepository subscriptionsRepository, IUnitOfWork unitOfWork)
    {
        _subscriptionsRepository = subscriptionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SubscriptionDisabledEvent notification, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(notification.SubscriptionId) ??
                            throw new InvalidOperationException("Subscription Not Found");
        
        foreach (var subsRoom in subscription.SubscriptionRooms)
            await _subscriptionsRepository.RemoveRoomFromSubscriptionAsync(subsRoom, cancellationToken);
            
        await _unitOfWork.CommitChangesAsync();
    }
}
