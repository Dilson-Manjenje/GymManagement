using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Trainers;
using GymManagement.Domain.Members;


namespace GymManagement.Application.Trainers.Commands.CreateTrainer;

public class  CreateTrainerCommandHandler : IRequestHandler<CreateTrainerCommand, ErrorOr<Guid>>
{
    private readonly IGymsRepository _gymsRepository;
    private readonly IMembersRepository _membersRepository;
    private readonly ITrainersRepository _trainerRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateTrainerCommandHandler(IUnitOfWork unitOfWork,
                                    IGymsRepository gymsRepository,
                                    ITrainersRepository trainerRepository,
                                    IMembersRepository membersRepository)
    {
        _unitOfWork = unitOfWork;
        _gymsRepository = gymsRepository;
        _trainerRepository = trainerRepository;
        _membersRepository = membersRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateTrainerCommand command, CancellationToken cancellationToken = default)
    {
              
        var member = await _membersRepository.GetByIdAsync(command.MemberId);
        if (member is null)
            return MemberErrors.MemberNotFound(command.MemberId);

        if (member.GymId is null || member.GymId.Value == Guid.Empty)
            return MemberErrors.MemberDontHaveGym(userName: member.UserName, memberId: member.Id);
        
        var gymId = member.GymId.Value;
        var gym = await _gymsRepository.GetByIdAsync(gymId, cancellationToken);
        if (gym is null)
            return GymErrors.GymNotFound(gymId);

        if (_trainerRepository.IsTrainerInGymAsync(gym.Id, command.MemberId))
            return TrainerErrors.TrainerAlreadyAddedToGym(member.Id);
            
        var trainer = new Trainer(
            name: command.Name,
            phone: command.Phone,
            email: command.Email,
            specialization: command.Specialization,
            gymId: gymId,
            memberId: command.MemberId
        );

        
        await _trainerRepository.AddAsync(trainer, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return trainer.Id;
    }
}