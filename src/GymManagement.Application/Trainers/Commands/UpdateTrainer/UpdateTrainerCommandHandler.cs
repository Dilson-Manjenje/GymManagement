using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Gyms;
using GymManagement.Domain.Trainers;
using GymManagement.Domain.Members;


namespace GymManagement.Application.Trainers.Commands.UpdateTrainer;

public class UpdateTrainerCommandHandler : IRequestHandler<UpdateTrainerCommand, ErrorOr<Guid>>
{
    private readonly ITrainersRepository _trainerRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateTrainerCommandHandler(IUnitOfWork unitOfWork,
                                    ITrainersRepository trainerRepository)
    {
        _unitOfWork = unitOfWork;
        _trainerRepository = trainerRepository;        
    }

    public async Task<ErrorOr<Guid>> Handle(UpdateTrainerCommand command, CancellationToken cancellationToken = default)
    {
        var trainer = await _trainerRepository.GetByIdAsync(command.Id);
        if (trainer is null)
            return TrainerErrors.TrainerNotFound(command.Id);

        var result = trainer.Update(
             name: command.Name,
             phone: command.Phone,
             email: command.Email,
             specialization: command.Specialization);

        if (result.IsError)
            return result.Errors;
        
        await _trainerRepository.AddAsync(trainer, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return trainer.Id;
    }
}