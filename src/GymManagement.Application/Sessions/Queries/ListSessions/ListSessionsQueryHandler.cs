using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.ListSessions;

public class ListSessionsQueryHandler : IRequestHandler<ListSessionsQuery, ErrorOr<IEnumerable<SessionDto>?>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public ListSessionsQueryHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<ErrorOr<IEnumerable<SessionDto>?>> Handle(ListSessionsQuery query, CancellationToken cancellationToken)
    {
        var sessions = await _sessionsRepository.ListAsync();

        return sessions?.Select(session => SessionDto.MapToDto(session)).ToList();
        
    }
}
