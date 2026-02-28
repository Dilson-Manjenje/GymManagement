using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Bookings.Events;
using MediatR;

namespace GymManagement.Application.Sessions.Events;


public class BookingCanceledEventHandler : INotificationHandler<BookingCanceledEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionsRepository _sessionsRepository;
    public BookingCanceledEventHandler(IUnitOfWork unitOfWork,
                                       ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(BookingCanceledEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Booking '{notification.BookingId}' canceled.");

        var session = await _sessionsRepository.GetByIdAsync(notification.SessionId) 
                     ?? throw new InvalidOperationException("Session Not Found");

        session.IncrementVacancy();

        // TODO: Add try/catch to handle RaceCondition on updating session.        
        await _sessionsRepository.UpdateAsync(session);
        await _unitOfWork.CommitChangesAsync();
        
        // TODO: Add Logs
        Console.WriteLine($"Session '{notification.SessionId}' vacancy restored, duo the booking cancellation.");
    }
}