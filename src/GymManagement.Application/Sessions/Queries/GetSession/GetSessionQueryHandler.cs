using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.GetSession;

public class GetSessionQueryHandler : IRequestHandler<GetSessionQuery, ErrorOr<SessionDto>>
{
    private readonly ISessionsRepository _sessionsRepository;
    
    public GetSessionQueryHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<ErrorOr<SessionDto>> Handle(GetSessionQuery query, CancellationToken cancellationToken)
    {
        var session = await _sessionsRepository.GetByIdAsync(query.SessionId);

        return (session is null)
            ? SessionErrors.SessionNotFound(query.SessionId)
            : SessionDto.MapToDto(session);
    }
}
