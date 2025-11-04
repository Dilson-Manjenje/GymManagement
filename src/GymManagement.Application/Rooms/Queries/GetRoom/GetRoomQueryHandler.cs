using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Rooms;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.GetRoom;

public class GetRoomQueryHandler : IRequestHandler<GetRoomQuery, ErrorOr<Room>>
{
    private readonly IRoomsRepository _roomsRepository;

    public GetRoomQueryHandler(IRoomsRepository roomsRepository)
    {
        _roomsRepository = roomsRepository;
    }
    
    public async Task<ErrorOr<Room>> Handle(GetRoomQuery request, CancellationToken cancellationToken)
    {
        var room = await _roomsRepository.GetByIdAsync(request.RoomId);

        return (room is null)
            ? RoomErrors.RoomNotFound(request.RoomId)
            : room;
    }
}
