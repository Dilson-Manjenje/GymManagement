using ErrorOr;
using GymManagement.Domain.Rooms;
using MediatR;

namespace GymManagement.Application.Rooms.Queries.GetRoomsByGym;

public record GetRoomsByGymQuery(Guid GymId): IRequest<ErrorOr<IEnumerable<Room>?>>;
