using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Rooms;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.GetRoomsByGym;

public class GetRoomsByGymQueryHandler : IRequestHandler<GetRoomsByGymQuery, ErrorOr<IEnumerable<Room>?>>
{
    private readonly IRoomsRepository _roomsRepository;
    private readonly IGymsRepository _gymsRepository;
    public GetRoomsByGymQueryHandler(IRoomsRepository roomsRepository,
                                     IGymsRepository gymsRepository)
    {
        _roomsRepository = roomsRepository;
        _gymsRepository = gymsRepository;
    }

    public async Task<ErrorOr<IEnumerable<Room>?>> Handle(GetRoomsByGymQuery query, CancellationToken cancellationToken)
    {
        var gym = await _gymsRepository.GetByIdAsync(query.GymId, cancellationToken);
        if (gym is null)
            return GymErrors.GymNotFound(query.GymId);
        
        var rooms = await _roomsRepository.GetRoomsByGymIdAsync(query.GymId, cancellationToken);

        return rooms?.ToList();
    }
}
