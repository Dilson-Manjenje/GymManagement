using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Rooms.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.ListRooms;

public class ListRoomsQueryHandler : IRequestHandler<ListRoomsQuery, ErrorOr<IEnumerable<RoomDetailsDto>?>>
{
    private readonly IRoomsRepository _roomsRepository;

    public ListRoomsQueryHandler(IRoomsRepository roomsRepository)
    {
        _roomsRepository = roomsRepository;
    }

    public async Task<ErrorOr<IEnumerable<RoomDetailsDto>?>> Handle(ListRoomsQuery query, CancellationToken cancellationToken)
    {
        var rooms = await _roomsRepository.ListAsync();        
        var dtos = rooms?.Select(r => RoomDetailsDto.MapToDto(r, r.Gym.Name))
                         .ToList();

        return dtos;
    }
}