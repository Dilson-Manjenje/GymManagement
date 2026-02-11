using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.ListSessionsByMember;

public class ListSessionsByMemberQueryHandler : IRequestHandler<ListSessionsByMemberQuery, ErrorOr<IEnumerable<SessionDto>?>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public ListSessionsByMemberQueryHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<ErrorOr<IEnumerable<SessionDto>?>> Handle(ListSessionsByMemberQuery query, CancellationToken cancellationToken)
    {
        var sessions = await _sessionsRepository.ListByMember(query.MemberId);
        return sessions?.Select(session => SessionDto.MapToDto(session)).ToList();
    }
    
}
