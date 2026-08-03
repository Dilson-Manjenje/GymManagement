using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Sessions;
using MediatR;
using OneOf.Types;

namespace GymManagement.Application.Rooms.Commands.DisableRoom;

public class DisableRoomCommandHandler : IRequestHandler<DisableRoomCommand, ErrorOr<Guid>>
{
    private readonly IRoomsRepository _roomsRepository;
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DisableRoomCommandHandler(IRoomsRepository roomsRepository,
                                     ISessionsRepository sessionsRepository,
                                     IUnitOfWork unitOfWork)
    {
        _roomsRepository = roomsRepository;
        _sessionsRepository = sessionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Guid>> Handle(DisableRoomCommand command, CancellationToken cancellationToken = default)
    {
        var room = await _roomsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (room is null)
            return RoomErrors.RoomNotFound(command.Id);

        var sessions = await _sessionsRepository.ListByRoomAsync(command.Id);
        var hasActiveSession = sessions is not null &&
                               sessions.Any(s => s.IsActive());
         
        if (hasActiveSession)
            return RoomErrors.CannotDisableRoomWithSessions(command.Id);

        var result = room.DisableRoom();

        if (result.IsError)
            return result.Errors;
        
        await _roomsRepository.UpdateAsync(room, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return room.Id;
    }
}
    