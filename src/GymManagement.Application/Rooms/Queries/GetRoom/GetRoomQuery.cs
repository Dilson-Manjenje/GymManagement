using ErrorOr;
using GymManagement.Application.Rooms.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.GetRoom;

public record GetRoomQuery(Guid Id): IRequest<ErrorOr<RoomDto>>;
