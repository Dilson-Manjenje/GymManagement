using ErrorOr;
using MediatR;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Gyms;
using GymManagement.Application.Common.Interfaces;


namespace GymManagement.Application.Rooms.Commands.CreateRoom;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, ErrorOr<Guid>>
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
    public async Task<ErrorOr<Guid>> Handle(CreateRoomCommand command, CancellationToken cancellationToken = default)
    {
        var gym = await _gymsRepository.GetByIdAsync(command.GymId, cancellationToken);
        if (gym is null)
            return GymErrors.GymNotFound(command.GymId);
        
        var room = new Room(
            name: command.Name,
            capacity: command.Capacity,
            gymId: command.GymId
        );

        await _roomsRepository.AddAsync(room, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return room.Id;
    }
}