using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Bookings;
using GymManagement.Domain.Subscriptions.Events;
using MediatR;

namespace GymManagement.Application.Subscriptions.Events;

public class SubscriptionDisabledEventHandler : INotificationHandler<SubscriptionDisabledEvent>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IBookingsRepository _bookingsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionDisabledEventHandler(IUnitOfWork unitOfWork,
                                            ISubscriptionsRepository subscriptionsRepository,
                                            IBookingsRepository bookingsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
        _unitOfWork = unitOfWork;
        _bookingsRepository = bookingsRepository;
    }

    public async Task Handle(SubscriptionDisabledEvent notification, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionsRepository.GetByIdAsync(notification.SubscriptionId) ??
                            throw new InvalidOperationException("Subscription Not Found");

        foreach (var subsRoom in subscription.SubscriptionRooms)
            await _subscriptionsRepository.RemoveRoomFromSubscriptionAsync(subsRoom, cancellationToken);

        var activeBookings = (await _bookingsRepository.ListByMemberAsync(subscription.MemberId))
                        ?.Where(b => b.Status == BookingStatus.Active);
        bool hasBookings = activeBookings is not null && activeBookings.Any();

        if (hasBookings)
        {
            foreach (var booking in activeBookings!)
            {
                var canceled = booking.Cancel();
                if (canceled.IsError)
                {
                    Console.WriteLine($"Error on cancelling Booking '{booking.Id}'. Error: {canceled.FirstError}");
                    continue;
                }
                Console.WriteLine($"Booking '{booking.Id}' canceled duo the subscription disabled.");
            }

            await _bookingsRepository.UpdateRangeAsync(activeBookings);
            await _unitOfWork.CommitChangesAsync();
        }
        
        await _unitOfWork.CommitChangesAsync();
    }
}
