using ErrorOr;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Trainers;
using MediatR;

namespace GymManagement.Application.Trainers.Commands.RemoveTrainer;

public class RemoveTrainerCommandHandler : IRequestHandler<RemoveTrainerCommand, ErrorOr<Updated>>
{
    private readonly ITrainersRepository _trainersRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RemoveTrainerCommandHandler(ITrainersRepository trainersRepository,
                                IUnitOfWork unitOfWork)
    {
        _trainersRepository = trainersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Updated>> Handle(RemoveTrainerCommand command, CancellationToken cancellationToken = default)
    {
        var trainer = await _trainersRepository.GetByIdAsync(command.Id, cancellationToken);

        if (trainer is null)
            return TrainerErrors.TrainerNotFound(command.Id);

        var result = trainer.RemoveTrainer();

        if (result.IsError)
            return TrainerErrors.CannotRemoveTrainerWithSessions($"{trainer.Name}(Phone: {trainer.Phone})");
        
        await _trainersRepository.UpdateAsync(trainer, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
    