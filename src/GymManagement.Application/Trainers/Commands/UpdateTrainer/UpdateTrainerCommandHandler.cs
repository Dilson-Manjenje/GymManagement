using ErrorOr;
using MediatR;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Trainers;


namespace GymManagement.Application.Trainers.Commands.UpdateTrainer;

public class UpdateTrainerCommandHandler : IRequestHandler<UpdateTrainerCommand, ErrorOr<Guid>>
{
    private readonly ITrainersRepository _trainersRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateTrainerCommandHandler(IUnitOfWork unitOfWork,
                                      ITrainersRepository trainersRepository)
    {
        _unitOfWork = unitOfWork;
        _trainersRepository = trainersRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(UpdateTrainerCommand command, CancellationToken cancellationToken = default)
    {
        var trainer = await _trainersRepository.GetByIdAsync(command.Id);
        if (trainer is null)
            return TrainerErrors.TrainerNotFound(command.Id);

        var phoneAlreadyUsed = await _trainersRepository.ExistsWithPhoneAsync(trainer.GymId, command.Phone, command.Id);
        if (phoneAlreadyUsed)
            return TrainerErrors.TrainerPhoneAlreadyExists(command.Phone);

        if (!string.IsNullOrEmpty(command.Email))
        {
            var emailAlreadyUsed = await _trainersRepository.ExistsWithEmailAsync(trainer.GymId, command.Email, command.Id);
            if (emailAlreadyUsed)
                return TrainerErrors.TrainerEmailAlreadyExists(command.Email!);
        }

        var result = trainer.Update(
             name: command.Name,
             phone: command.Phone,
             email: command.Email?.ToLower(),
             specialization: command.Specialization);

        if (result.IsError)
            return result.Errors;
        
        await _trainersRepository.UpdateAsync(trainer, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return trainer.Id;
    }
}