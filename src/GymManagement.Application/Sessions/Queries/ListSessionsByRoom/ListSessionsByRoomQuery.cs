using ErrorOr;
using GymManagement.Application.Sessions.Queries.Dtos;
using GymManagement.Domain.Sessions;
using MediatR;

namespace GymManagement.Application.Sessions.Queries.ListSessionsByRoom;

public record ListSessionsByRoomQuery(Guid RoomId): IRequest<ErrorOr<IEnumerable<SessionDto>?>>;
