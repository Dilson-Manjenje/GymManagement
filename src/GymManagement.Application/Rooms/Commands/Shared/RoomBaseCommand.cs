using ErrorOr;
using MediatR;

namespace GymManagement.Application.Rooms.Commands.Shared;

public abstract record RoomBaseCommand(string Name,
                                        int Capacity) : IRequest<ErrorOr<Guid>>;