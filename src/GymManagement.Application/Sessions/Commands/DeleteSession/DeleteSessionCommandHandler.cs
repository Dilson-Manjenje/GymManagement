using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Sessions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GymManagement.Application.Sessions.Commands.DeleteSession;

public class DeleteSessionCommandHandler : IRequestHandler<DeleteSessionCommand, ErrorOr<Unit>>
{
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSessionCommandHandler(IUnitOfWork unitOfWork, ISessionsRepository sessionsRepository)
    {
        _unitOfWork = unitOfWork;
        _sessionsRepository = sessionsRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteSessionCommand command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (session is null)
            return SessionErrors.SessionNotFound(command.Id);

        if (SessionStatus.NonCancelableStatus.Contains(session.Status))
            return SessionErrors.CantChangeSession(command.Id);

        // TODO: Check if has active booking, and not allow
        
        // var result = session.DeleteSession();        
        var result = session.Cancel();
        if (result.IsError)
            return result.Errors;

        await _sessionsRepository.RemoveAsync(session);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
    