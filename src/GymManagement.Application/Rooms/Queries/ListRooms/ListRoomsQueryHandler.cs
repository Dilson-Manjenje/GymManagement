using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Rooms;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.ListRooms;

public class ListRoomsQueryHandler : IRequestHandler<ListRoomsQuery, ErrorOr<IEnumerable<Room>?>>
{
    private readonly IRoomsRepository _roomsRepository;

    public ListRoomsQueryHandler(IRoomsRepository roomsRepository)
    {
        _roomsRepository = roomsRepository;
    }

    public async Task<ErrorOr<IEnumerable<Room>?>> Handle(ListRoomsQuery query, CancellationToken cancellationToken)
    {
        var rooms = await _roomsRepository.ListAsync();

        return rooms?.ToList();
    }
}