using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Gyms;


namespace GymManagement.Application.Rooms.Commands.CreateRoom;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, ErrorOr<Room>>
{
    private readonly IRoomsRepository _roomsRepository;
    private readonly IGymsRepository _gymsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateRoomCommandHandler(IRoomsRepository IRoomsRepository,
                                    IGymsRepository gymsRepository,
                                    IUnitOfWork unitOfWork)
    {
        _roomsRepository = IRoomsRepository;
        _gymsRepository = gymsRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<ErrorOr<Room>> Handle(CreateRoomCommand request, CancellationToken cancellationToken = default)
    {
        var gym = await _gymsRepository.GetByIdAsync(request.GymId, cancellationToken);
        if (gym is null)
            return GymErrors.GymNotFound(request.GymId);
        
        var validator = new CreateRoomCommandValidator(_roomsRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => Error.Validation(code: e.PropertyName, description: e.ErrorMessage))
                .ToList();
            return errors;
        }
        // TODO: Check if RoomId is already in the Gym by Name, Type
        
        var room = new Room(
            name: request.Name,
            capacity: request.Capacity,
            gymId: request.GymId
        );

        await _roomsRepository.AddAsync(room, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return room;
    }
}