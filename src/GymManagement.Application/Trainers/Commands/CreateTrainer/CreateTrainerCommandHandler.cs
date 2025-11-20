using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Trainers;
using GymManagement.Domain.Admins;


namespace GymManagement.Application.Trainers.Commands.CreateTrainer;

public class  CreateTrainerCommandHandler : IRequestHandler<CreateTrainerCommand, ErrorOr<Trainer>>
{
    private readonly IGymsRepository _gymsRepository;
    private readonly IAdminsRepository _adminsRepository;
    private readonly ITrainersRepository _trainerRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateTrainerCommandHandler(IUnitOfWork unitOfWork,
                                    IGymsRepository gymsRepository,
                                    ITrainersRepository trainerRepository,
                                    IAdminsRepository adminsRepository)
    {
        _unitOfWork = unitOfWork;
        _gymsRepository = gymsRepository;
        _trainerRepository = trainerRepository;
        _adminsRepository = adminsRepository;
    }

    public async Task<ErrorOr<Trainer>> Handle(CreateTrainerCommand command, CancellationToken cancellationToken = default)
    {
       
        var gym = await _gymsRepository.GetByIdAsync(command.GymId, cancellationToken);
        if (gym is null)
            return GymErrors.GymNotFound(command.GymId);

        var admin = await _adminsRepository.GetByIdAsync(command.AdminId);
        if (admin is null)
            return AdminErrors.UserNotFound(command.AdminId);
        
        // TODO: Check if Trainer is already in the Gym
        var trainer = new Trainer(
            name: command.Name,
            phone: command.Phone,
            email: command.Email,
            specialization: command.Specialization,
            gymId: command.GymId,
            adminId: command.AdminId
        );

        await _trainerRepository.AddAsync(trainer, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return trainer;
    }
}