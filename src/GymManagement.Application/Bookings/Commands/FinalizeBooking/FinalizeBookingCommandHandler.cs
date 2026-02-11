using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Bookings;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Bookings.Commands.FinalizeBooking;

public class FinalizeBookingCommandHandler : IRequestHandler<FinalizeBookingCommand, ErrorOr<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingsRepository _bookingsRepository;
    public FinalizeBookingCommandHandler(IUnitOfWork unitOfWork,
                                       IBookingsRepository bookingsRepository,
                                       ISessionsRepository sessionsRepository)
    {
        _bookingsRepository = bookingsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(FinalizeBookingCommand command, CancellationToken cancellationToken)
    {
        var booking = await _bookingsRepository.GetByIdAsync(command.BookingId);
        if (booking is null)
            return BookingErrors.BookingNotFound(command.BookingId);

        // Finalize Booking only if session is: Finalized
        // TODO: On Finalize Session then Evenctually finalize all active bookings
        if ( booking.Session.Status != SessionStatus.Finalized)
            return BookingErrors.InvalidSessionsStatus(id: booking.Session.Id, statusName: booking.Session.Status.Name);

        var result = booking.Finalize();
        if (result.IsError)
            return result.Errors;

        await _bookingsRepository.UpdateAsync(booking);
        await _unitOfWork.CommitChangesAsync();
        
        return booking.Id;
    }
}

