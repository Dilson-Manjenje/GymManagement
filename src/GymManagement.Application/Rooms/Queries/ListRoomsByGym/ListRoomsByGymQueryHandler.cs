using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Rooms.Queries.Dtos;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.ListRoomsByGym;

public class ListRoomsByGymQueryHandler : IRequestHandler<ListRoomsByGymQuery, ErrorOr<IEnumerable<RoomDetailsDto>?>>
{
    private readonly IRoomsRepository _roomsRepository;
    private readonly IGymsRepository _gymsRepository;
    public ListRoomsByGymQueryHandler(IRoomsRepository roomsRepository,
                                     IGymsRepository gymsRepository)
    {
        _roomsRepository = roomsRepository;
        _gymsRepository = gymsRepository;
    }

    public async Task<ErrorOr<IEnumerable<RoomDetailsDto>?>> Handle(ListRoomsByGymQuery query, CancellationToken cancellationToken)
    {
        var gym = await _gymsRepository.GetByIdAsync(query.GymId, cancellationToken);
        if (gym is null)
            return GymErrors.GymNotFound(query.GymId);
        
        var rooms = await _roomsRepository.ListByGymAsync(query.GymId, cancellationToken);

        var dtos = rooms?.Select(r => RoomDetailsDto.MapToDto(r, r.Gym.Name))
                         .ToList();
        
        return dtos;
    }
}
