using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Bookings.Events;
using MediatR;

namespace GymManagement.Application.Sessions.Events;

public class BookingCreatedEventHandler : INotificationHandler<BookingCreatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionsRepository _sessionsRepository;
    public BookingCreatedEventHandler(IUnitOfWork unitOfWork,
                                       ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Booking '{notification.BookingId}' created.");

        var session = await _sessionsRepository.GetByIdAsync(notification.SessionId) 
                     ?? throw new InvalidOperationException("Session Not Found");

        session.DecrementVacancy();

        // TODO: Add try/catch to handle RaceCondition on updating session.        
        await _sessionsRepository.UpdateAsync(session);
        await _unitOfWork.CommitChangesAsync();

    }
}