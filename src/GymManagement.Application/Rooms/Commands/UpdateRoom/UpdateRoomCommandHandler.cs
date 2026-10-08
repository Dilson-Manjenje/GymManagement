using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Rooms;
using MediatR;

namespace GymManagement.Application.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, ErrorOr<Guid>>
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

    public async Task<ErrorOr<Guid>> Handle(UpdateRoomCommand command, CancellationToken cancellationToken)
    {
        var room = await _roomsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (room is null)
            return RoomErrors.RoomNotFound(command.Id);
            
        var nameAlreadyExist = await _roomsRepository.ExistWithNameAsync(room.GymId, command.Name, room.Id);
        if (nameAlreadyExist)
            return RoomErrors.RoomNameAlreadyExist(command.Name);

        var result = room.UpdateRoom(command.Name,
                                     command.Capacity);

        if (result.IsError)
            return result.Errors;
        
        await _roomsRepository.UpdateAsync(room, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return room.Id;
    }
}