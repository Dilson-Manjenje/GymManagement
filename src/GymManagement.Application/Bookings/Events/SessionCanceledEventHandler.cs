using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Sessions.Events;
using MediatR;

namespace GymManagement.Application.Bookings.Events;

public class SessionCanceledEventHandler : INotificationHandler<SessionCanceledEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IBookingsRepository _bookingsRepository;

    public SessionCanceledEventHandler(IUnitOfWork unitOfWork,
                                       IBookingsRepository bookingsRepository,
                                       ISessionsRepository sessionsRepository)
    {
        _bookingsRepository = bookingsRepository;
        _sessionsRepository = sessionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SessionCanceledEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Session '{notification.SessionId}' was canceled.");

        var session = await _sessionsRepository.GetByIdAsync(notification.SessionId) 
                     ?? throw new InvalidOperationException("Session Not Found");
        
        var activeBookings = await _bookingsRepository.ListActiveBookingsBySessionAsync(notification.SessionId);
        var hasBookings = activeBookings is not null && activeBookings.Any();

        if (hasBookings)
        {
            foreach (var booking in activeBookings!)
            {
                var canceled = booking.Cancel();
                if (canceled.IsError)
                {
                    // TODO: log errors
                    Console.WriteLine($"Error on cancelling Booking '{booking.Id}'. Error: {canceled.FirstError}");
                    continue;
                }
                Console.WriteLine($"Booking '{booking.Id}'canceled duo the session canceletion.");
                    
                // booking.Session.IncrementVacancy();
            }

            await _bookingsRepository.UpdateRangeAsync(activeBookings);
            await _unitOfWork.CommitChangesAsync();
        }
    }
}