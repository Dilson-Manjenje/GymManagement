using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Rooms.Queries.Dtos;
using GymManagement.Domain.Rooms;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.GetRoom;

public class GetRoomQueryHandler : IRequestHandler<GetRoomQuery, ErrorOr<RoomDto>>
{
    private readonly IRoomsRepository _roomsRepository;

    public GetRoomQueryHandler(IRoomsRepository roomsRepository)
    {
        _roomsRepository = roomsRepository;
    }
    
    public async Task<ErrorOr<RoomDto>> Handle(GetRoomQuery query, CancellationToken cancellationToken)
    {
        var room = await _roomsRepository.GetByIdAsync(query.RoomId);

        return (room is null)
            ? RoomErrors.RoomNotFound(query.RoomId)
            : RoomDto.MapToDto(room);
    }
}
