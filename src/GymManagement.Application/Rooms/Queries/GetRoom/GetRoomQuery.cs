using ErrorOr;
using GymManagement.Domain.Rooms;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.GetRoom;

public record GetRoomQuery(Guid RoomId): IRequest<ErrorOr<Room>>;
