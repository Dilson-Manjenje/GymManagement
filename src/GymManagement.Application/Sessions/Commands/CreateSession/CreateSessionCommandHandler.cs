using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Sessions.Shared;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Sessions;
using GymManagement.Domain.Trainers;
using MediatR;

namespace GymManagement.Application.Sessions.Commands.CreateSession;

public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, ErrorOr<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IRoomsRepository _roomsRepository;
    private readonly ITrainersRepository _trainersRepository;
    public CreateSessionCommandHandler(IUnitOfWork unitOfWork,
                                       ISessionsRepository sessionsRepository,
                                       IRoomsRepository roomsRepository,
                                       ITrainersRepository trainersRepository)
    {
        _unitOfWork = unitOfWork;
        _sessionsRepository = sessionsRepository;
        _roomsRepository = roomsRepository;
        _trainersRepository = trainersRepository;
    }
    public async Task<ErrorOr<Guid>> Handle(CreateSessionCommand command, CancellationToken cancellationToken)
    {        
        var room = await _roomsRepository.GetByIdAsync(command.RoomId);
        if (room is null)
            return RoomErrors.RoomNotFound(command.RoomId);

        var trainer = await _trainersRepository.GetByIdAsync(command.TrainerId);
        if (trainer is null)
            return TrainerErrors.TrainerNotFound(command.TrainerId);

        if (room.GymId != trainer.GymId)
            return SessionErrors.TrainerNotInTheSameGym(trainerId: trainer.Id);

        // if (DateTime.Now.TimeOfDay >= TimeSpan.FromHours(21))
        //     return SessionErrors.CannotCreateAfterBusinessHours;
            
        var startTime = command.StartDate ?? DateTime.Now.AddMinutes(10); // TODO: Inject Time Zone Provider
        var endTime = command.EndDate ?? startTime.AddHours(1);

        if (await _roomsRepository.RoomHasOverlappingSession(command.RoomId, startTime, endTime))
            return RoomErrors.RoomHasOverlappingSession();
           
        var session = new Session(roomId: command.RoomId,
                                  trainerId: command.TrainerId,
                                  title: command.Title,
                                  capacity: room.Capacity,
                                  vacancy: room.Capacity, 
                                  startDate: startTime,
                                  endDate: endTime );

        await _sessionsRepository.AddAsync(session);
        await _unitOfWork.CommitChangesAsync();

        return session.Id;        
    }
}