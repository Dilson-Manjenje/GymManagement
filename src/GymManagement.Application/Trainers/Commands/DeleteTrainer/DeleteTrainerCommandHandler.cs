using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Trainers;
using MediatR;

namespace GymManagement.Application.Trainers.Commands.DeleteTrainer;

public class DeleteTrainerCommandHandler : IRequestHandler<DeleteTrainerCommand, ErrorOr<Unit>>
{
    private readonly ITrainersRepository _trainersRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteTrainerCommandHandler(ITrainersRepository trainersRepository,
                                IUnitOfWork unitOfWork)
    {
        _trainersRepository = trainersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteTrainerCommand command, CancellationToken cancellationToken = default)
    {
        var trainer = await _trainersRepository.GetByIdAsync(command.Id, cancellationToken);

        if (trainer is null)
            return TrainerErrors.TrainerNotFound(command.Id);

        // TODO: Check if has active sessions
        var result = trainer.RemoveTrainer();

        if (result.IsError)
            return result.Errors;
        
        await _trainersRepository.RemoveAsync(trainer, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
    