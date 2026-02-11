using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.ListSessionsByGym;

public class ListSessionsByGymQueryHandler : IRequestHandler<ListSessionsByGymQuery, ErrorOr<IEnumerable<SessionDto>?>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public ListSessionsByGymQueryHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<ErrorOr<IEnumerable<SessionDto>?>> Handle(ListSessionsByGymQuery query, CancellationToken cancellationToken)
    {
        var sessions = await _sessionsRepository.ListByGymAsync(query.GymId);

        return sessions?.Select(session => SessionDto.MapToDto(session)).ToList();
        
    }
}
