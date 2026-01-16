using ErrorOr;
using GymManagement.Application.Rooms.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.ListRooms;

public record ListRoomsQuery(): IRequest<ErrorOr<IEnumerable<RoomDto>?>>;
