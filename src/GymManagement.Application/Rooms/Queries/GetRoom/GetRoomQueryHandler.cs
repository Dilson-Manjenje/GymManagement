using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Rooms.Queries.Dtos;
using GymManagement.Domain.Rooms;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.GetRoom;

public class GetRoomQueryHandler : IRequestHandler<GetRoomQuery, ErrorOr<RoomDetailsDto>>
{
    private readonly IRoomsRepository _roomsRepository;

    public GetRoomQueryHandler(IRoomsRepository roomsRepository)
    {
        _roomsRepository = roomsRepository;
    }
    
    public async Task<ErrorOr<RoomDetailsDto>> Handle(GetRoomQuery query, CancellationToken cancellationToken)
    {
        var room = await _roomsRepository.GetWithDetails(query.RoomId);

        return (room is null)
            ? RoomErrors.RoomNotFound(query.RoomId)
            : room;
    }
}
