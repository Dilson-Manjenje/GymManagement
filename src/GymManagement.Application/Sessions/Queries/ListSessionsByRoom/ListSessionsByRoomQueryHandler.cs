using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.ListSessionsByRoom;

public class ListSessionsByRoomQueryHandler : IRequestHandler<ListSessionsByRoomQuery, ErrorOr<IEnumerable<SessionDto>?>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public ListSessionsByRoomQueryHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<ErrorOr<IEnumerable<SessionDto>?>> Handle(ListSessionsByRoomQuery query, CancellationToken cancellationToken)
    {
        var sessions = await _sessionsRepository.ListByRoomAsync(query.RoomId);

        return sessions?.Select(session => SessionDto.MapToDto(session)).ToList();
        
    }
}
