using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.ListSessionsByTrainer;

public class ListSessionsByTrainerQueryHandler : IRequestHandler<ListSessionsByTrainerQuery, ErrorOr<IEnumerable<SessionDto>?>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public ListSessionsByTrainerQueryHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<ErrorOr<IEnumerable<SessionDto>?>> Handle(ListSessionsByTrainerQuery query, CancellationToken cancellationToken)
    {
        var sessions = await _sessionsRepository.ListByTrainer(query.TrainerId);

        return sessions?.Select(session => SessionDto.MapToDto(session)).ToList();
        
    }
}
