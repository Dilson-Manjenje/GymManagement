using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Bookings;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Bookings.Commands.CancelBooking;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, ErrorOr<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingsRepository _bookingsRepository;
    private readonly ISessionsRepository _sessionsRepository;

    public CancelBookingCommandHandler(IUnitOfWork unitOfWork,
                                       IBookingsRepository bookingsRepository,
                                       ISessionsRepository sessionsRepository)
    {
        _bookingsRepository = bookingsRepository;
        _sessionsRepository = sessionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(CancelBookingCommand command, CancellationToken cancellationToken)
    {
        var booking = await _bookingsRepository.GetByIdAsync(command.BookingId);
        if (booking is null)
            return BookingErrors.BookingNotFound(command.BookingId);

        var session = await _sessionsRepository.GetByIdAsync(booking.SessionId);
        if (session is null)
            return SessionErrors.SessionNotFound(booking.SessionId);

        if (session.Status == SessionStatus.Finalized)
            return BookingErrors.InvalidSessionsStatus(id: booking.Session.Id, statusName: session.Status.Name);
        
        var canceled = booking.Cancel();
        if (canceled.IsError)
            return canceled.Errors;
        
        session.IncrementVacancy();
                     
        await _bookingsRepository.UpdateAsync(booking);
        await _sessionsRepository.UpdateAsync(session);
        await _unitOfWork.CommitChangesAsync();
        
        return booking.Id;
    }
}

