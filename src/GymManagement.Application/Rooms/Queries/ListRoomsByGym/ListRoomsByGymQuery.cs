using ErrorOr;
using GymManagement.Application.Rooms.Queries.Dtos;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.ListRoomsByGym;

public record ListRoomsByGymQuery(Guid GymId): IRequest<ErrorOr<IEnumerable<RoomDto>?>>;
