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

    public async Task<ErrorOr<Room>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var gym = await _gymsRepository.GetByIdAsync(request.GymId, cancellationToken);
        if (gym is null)
            return GymErrors.GymNotFound(request.GymId);
        
        var validator = new UpdateRoomCommandValidator(_roomsRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => Error.Validation(code: e.PropertyName, description: e.ErrorMessage))
                .ToList();
            return errors;
        }
        
        var room = await _roomsRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (room is null)
            return RoomErrors.RoomNotFound(request.Id);

        var result = room.UpdateRoom(request.Name,
                                     request.Capacity,
                                     request.GymId);

        if (result.IsError)
            return result.Errors;
        
        await _roomsRepository.UpdateAsync(room, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return room;
    }
}