using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using MediatR;

namespace GymManagement.Application.Gyms.Commands.DeleteGym;

public class DeleteGymCommandHandler : IRequestHandler<DeleteGymCommand, ErrorOr<Unit>>
{
    private readonly IGymsRepository _gymsRepository;
    private readonly IRoomsRepository _roomsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteGymCommandHandler(IGymsRepository gymsRepository,
                                   IRoomsRepository roomsRepository,
                                   IUnitOfWork unitOfWork)
    {
        _gymsRepository = gymsRepository;
        _roomsRepository = roomsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteGymCommand command, CancellationToken cancellationToken = default)
    {
        var gym = await _gymsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (gym is null)
            return GymErrors.GymNotFound(command.Id);

        var rooms = await _roomsRepository.ListByGymAsync(command.Id);

        if (rooms is not null && rooms.Any())
            return GymErrors.CannotDeleteGymWithRooms(command.Id);

        await _gymsRepository.RemoveAsync(gym, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
    