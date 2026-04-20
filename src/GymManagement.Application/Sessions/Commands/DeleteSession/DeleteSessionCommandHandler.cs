using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Sessions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GymManagement.Application.Sessions.Commands.DeleteSession;

public class DeleteSessionCommandHandler : IRequestHandler<DeleteSessionCommand, ErrorOr<Unit>>
{
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IBookingsRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSessionCommandHandler(IUnitOfWork unitOfWork, ISessionsRepository sessionsRepository, IBookingsRepository bookingRepository)
    {
        _unitOfWork = unitOfWork;
        _sessionsRepository = sessionsRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteSessionCommand command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (session is null)
            return SessionErrors.SessionNotFound(command.Id);

        if (SessionStatus.NonCancelableStatus.Contains(session.Status))
            return SessionErrors.CantChangeSession(command.Id);

        var bookings = await _bookingRepository.ListBookingsBySessionAsync(sessionId: session.Id);

        if (bookings is not null && bookings.Any())
            return SessionErrors.CantCancelSessionWithBooking(id: session.Id);

        await _sessionsRepository.RemoveAsync(session);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
    