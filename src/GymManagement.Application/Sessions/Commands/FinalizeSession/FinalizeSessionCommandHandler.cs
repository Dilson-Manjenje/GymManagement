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

        var result = session.Finalize();

        if (result.IsError)
            return result.Errors;

        await _sessionsRepository.UpdateAsync(session);
        await _unitOfWork.CommitChangesAsync();
        // Bookings are finalized in Eventual Consistency manner by SessionFinalizedEvent    
        
        return session.Id;
    }
}

