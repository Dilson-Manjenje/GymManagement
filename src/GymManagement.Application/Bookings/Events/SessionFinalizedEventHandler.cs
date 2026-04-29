using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Sessions.Events;
using MediatR;

namespace GymManagement.Application.Bookings.Events;

public class SessionFinalizedEventHandler : INotificationHandler<SessionFinalizedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IBookingsRepository _bookingsRepository;

    public SessionFinalizedEventHandler(IUnitOfWork unitOfWork,
                                       IBookingsRepository bookingsRepository,
                                       ISessionsRepository sessionsRepository)
    {
        _bookingsRepository = bookingsRepository;
        _sessionsRepository = sessionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SessionFinalizedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Session '{notification.SessionId}' was completed.");

        var session = await _sessionsRepository.GetByIdAsync(notification.SessionId) 
                     ?? throw new InvalidOperationException("Session Not Found");
        
        var activeBookings = await _bookingsRepository.ListActiveBookingsBySessionAsync(notification.SessionId);
        var hasBookings = activeBookings is not null && activeBookings.Any();

        if (hasBookings)
        {
            foreach (var booking in activeBookings!)
            {
                var finalized = booking.Finalize();
                if (finalized.IsError)
                {
                    // TODO: log error
                    Console.WriteLine($"Error on finalizing Booking '{booking.Id}'. Error: {finalized.Errors}");
                    continue;         
                }

                Console.WriteLine($"Booking '{booking.Id}' complete duo the session finalization.");                
            }

            await _bookingsRepository.UpdateRangeAsync(activeBookings);
            await _unitOfWork.CommitChangesAsync();
        }
    }
}