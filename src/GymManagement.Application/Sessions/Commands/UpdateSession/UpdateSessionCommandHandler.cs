using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Sessions;
using GymManagement.Domain.Trainers;
using MediatR;

namespace GymManagement.Application.Sessions.Commands.UpdateSession;

public class UpdateSessionCommandHandler : IRequestHandler<UpdateSessionCommand, ErrorOr<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionsRepository _sessionsRepository;
    private readonly IRoomsRepository _roomsRepository;
    private readonly ITrainersRepository _trainersRepository;
    public UpdateSessionCommandHandler(IUnitOfWork unitOfWork,
                                       ISessionsRepository sessionsRepository,
                                       IRoomsRepository roomsRepository,
                                       ITrainersRepository trainersRepository)
    {
        _unitOfWork = unitOfWork;
        _sessionsRepository = sessionsRepository;
        _roomsRepository = roomsRepository;
        _trainersRepository = trainersRepository;
    }
    public async Task<ErrorOr<Guid>> Handle(UpdateSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await _sessionsRepository.GetByIdAsync(command.Id);
        if (session is null)
            return SessionErrors.SessionNotFound(command.Id);

        if(SessionStatus.NonCancelableStatus.Contains(session.Status))
            return SessionErrors.CantChangeSession(command.Id);

        var newRoom = await _roomsRepository.GetByIdAsync(command.RoomId);
        if (newRoom is null)
            return RoomErrors.RoomNotFound(command.RoomId);

        if (newRoom.GymId != session.Room.GymId) 
            return SessionErrors.CantChangeGym();

        var trainer = await _trainersRepository.GetByIdAsync(command.TrainerId);
        if (trainer is null)
            return TrainerErrors.TrainerNotFound(command.TrainerId);

        if (newRoom.GymId != trainer.GymId)
            return SessionErrors.TrainerNotInTheSameGym(trainerId: trainer.Id);
        
        var startTime = command.StartDate ?? session.StartDate;
        var endTime = command.EndDate ?? session.EndDate;

        // TODO: Should allow update, if the overlapping is on the same session?
         
        if (command.StartDate.HasValue && command.EndDate.HasValue)
            if (await _roomsRepository.RoomHasOverlappingSession(roomId: command.RoomId,
                                                           command.StartDate.Value,
                                                           command.EndDate.Value))

                return RoomErrors.RoomHasOverlappingSession();
            
        var result = session.UpdateSession(roomId: command.RoomId,
                              trainerId: command.TrainerId,
                              title: command.Title,
                              startDate: command.StartDate ?? session.StartDate,
                              endDate:  command.EndDate ?? session.EndDate);

        if (result.IsError)
            return result.Errors;
        
        await _sessionsRepository.UpdateAsync(session);
        await _unitOfWork.CommitChangesAsync();
        
        return session.Id;        
    }
}