using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Bookings;
using GymManagement.Domain.Sessions;
using MediatR;
using OneOf.Types;

namespace GymManagement.Application.Sessions.Commands.FinalizeSession;

public class FinalizeSessionCommandHandler : IRequestHandler<FinalizeSessionCommand, ErrorOr<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IBookingsRepository _bookingsRepository;

    public FinalizeSessionCommandHandler(IUnitOfWork unitOfWork,
                                       IBookingsRepository bookingsRepository,
                                       ISessionsRepository sessionsRepository)
    {
        _bookingsRepository = bookingsRepository;
        _sessionsRepository = sessionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(FinalizeSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await _sessionsRepository.GetByIdAsync(command.SessionId);
        if (session is null)
            return SessionErrors.SessionNotFound(command.SessionId);

        var activeBookings = await _bookingsRepository.ListActiveBookingsBySessionAsync(command.SessionId);
        var hasBookings = activeBookings is not null && activeBookings.Any();

        if (hasBookings)
        {
            foreach (var booking in activeBookings!)
            {
                var finalized = booking.Finalize();
                if (finalized.IsError)
                    return finalized.Errors;
            }
        }

        var result = session.Finalize();

        if (result.IsError)
            return result.Errors;

        if (hasBookings)
            await _bookingsRepository.UpdateRangeAsync(activeBookings!);
            
        await _sessionsRepository.UpdateAsync(session);
        await _unitOfWork.CommitChangesAsync();
        
        return session.Id;
    }
}

