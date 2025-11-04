using ErrorOr;
using MediatR;

namespace GymManagement.Application.Rooms.Commands.DisableRoom;
public record DisableRoomCommand(Guid Id) : IRequest<ErrorOr<Updated>>;