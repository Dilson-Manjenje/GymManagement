using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Bookings;
using GymManagement.Domain.Sessions;
using MediatR;
using OneOf.Types;

namespace GymManagement.Application.Sessions.Commands.CancelSession;

public class CancelSessionCommandHandler : IRequestHandler<CancelSessionCommand, ErrorOr<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IBookingsRepository _bookingsRepository;

    public CancelSessionCommandHandler(IUnitOfWork unitOfWork,
                                       IBookingsRepository bookingsRepository,
                                       ISessionsRepository sessionsRepository)
    {
        _bookingsRepository = bookingsRepository;
        _sessionsRepository = sessionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(CancelSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await _sessionsRepository.GetByIdAsync(command.SessionId);
        if (session is null)
            return SessionErrors.SessionNotFound(command.SessionId);

        // TODO: Cancel bookings using Eventual Consistency 
        // var activeBookings = await _bookingsRepository.ListActiveBookingsBySessionAsync(command.SessionId);
        // var hasBookings = activeBookings is not null && activeBookings.Any();
        
        // if (hasBookings)
        // {
        //     foreach (var booking in activeBookings!)
        //     {
        //         var canceled = booking.Cancel();
        //         if (canceled.IsError)
        //             return canceled.Errors;

        //         session.IncrementVacancy();
        //     }
        // }

        var result = session.Cancel();

        if (result.IsError)
            return result.Errors;

        // if (hasBookings)
        //     await _bookingsRepository.UpdateRangeAsync(activeBookings!);

        await _sessionsRepository.UpdateAsync(session);
        await _unitOfWork.CommitChangesAsync();
        
        return session.Id;
    }
}

