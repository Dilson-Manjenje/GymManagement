using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.ListUpComingSessions;

public class ListUpComingSessionsQueryHandler : IRequestHandler<ListUpComingSessionsQuery, ErrorOr<IEnumerable<SessionDto>?>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public ListUpComingSessionsQueryHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<ErrorOr<IEnumerable<SessionDto>?>> Handle(ListUpComingSessionsQuery query, CancellationToken cancellationToken)
    {
        var sessions = await _sessionsRepository.ListUpCommingSessions(query.GymId);

        return sessions?.Select(session => SessionDto.MapToDto(session)).ToList();
        
    }
}
