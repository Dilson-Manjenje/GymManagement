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
    private readonly ITrainersRepository _trainersRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateTrainerCommandHandler(IUnitOfWork unitOfWork,
                                    IGymsRepository gymsRepository,
                                    ITrainersRepository trainersRepository,
                                    IMembersRepository membersRepository)
    {
        _unitOfWork = unitOfWork;
        _gymsRepository = gymsRepository;
        _trainersRepository = trainersRepository;
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

        var isTrainerAddedToGym = await _trainersRepository.IsTrainerInGymAsync(gym.Id, command.MemberId);
        if (isTrainerAddedToGym)
            return TrainerErrors.TrainerAlreadyAddedToGym(member.Id);

        
        var phoneAlreadyUsed = await _trainersRepository.ExistsWithPhoneAsync(member.GymId.Value, command.Phone, null);
        if (phoneAlreadyUsed)
            return TrainerErrors.TrainerPhoneAlreadyExists(command.Phone);

        if (!string.IsNullOrEmpty(command.Email))
        {
           var emailAlreadyUsed = await _trainersRepository.ExistsWithEmailAsync(member.GymId.Value, command.Email!, null);            
            if (emailAlreadyUsed)
                return TrainerErrors.TrainerEmailAlreadyExists(command.Email!);
        }
    
                          
        var trainer = new Trainer(
            name: command.Name,
            phone: command.Phone,
            email: command.Email?.ToLower(),
            specialization: command.Specialization,
            gymId: gymId,
            memberId: command.MemberId
        );

        
        await _trainersRepository.AddAsync(trainer, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return trainer.Id;
    }
}