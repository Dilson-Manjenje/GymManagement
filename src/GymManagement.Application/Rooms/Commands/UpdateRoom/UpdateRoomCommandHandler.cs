using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Rooms;
using MediatR;

namespace GymManagement.Application.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, ErrorOr<Room>>
{
    private readonly IRoomsRepository _roomsRepository;
    private readonly IGymsRepository _gymsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateRoomCommandHandler(IRoomsRepository roomsRepository,
                                    IGymsRepository gymsRepository,
                                    IUnitOfWork unitOfWork)
    {
        _roomsRepository = roomsRepository;
        _gymsRepository = gymsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Room>> Handle(UpdateRoomCommand command, CancellationToken cancellationToken)
    {
        var gym = await _gymsRepository.GetByIdAsync(command.GymId, cancellationToken);
        if (gym is null)
            return GymErrors.GymNotFound(command.GymId);
        
        var room = await _roomsRepository.GetByIdAsync(command.Id, cancellationToken);
        
        if (room is null)
            return RoomErrors.RoomNotFound(command.Id);

        var result = room.UpdateRoom(command.Name,
                                     command.Capacity,
                                     command.GymId);

        if (result.IsError)
            return result.Errors;
        
        await _roomsRepository.UpdateAsync(room, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return room;
    }
}