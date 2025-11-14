using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Rooms;
using MediatR;

namespace GymManagement.Application.Rooms.Commands.DisableRoom;

public class DisableRoomCommandHandler : IRequestHandler<DisableRoomCommand, ErrorOr<Updated>>
{
    private readonly IRoomsRepository _roomsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DisableRoomCommandHandler(IRoomsRepository roomsRepository,
                                IUnitOfWork unitOfWork)
    {
        _roomsRepository = roomsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Updated>> Handle(DisableRoomCommand command, CancellationToken cancellationToken = default)
    {
        var room = await _roomsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (room is null)
            return RoomErrors.RoomNotFound(command.Id);

        var result = room.DisableRoom();
        if (result.IsError)
            return RoomErrors.CannotDisableRoomWithSessions(command.Id);
        
        await _roomsRepository.UpdateAsync(room, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
    