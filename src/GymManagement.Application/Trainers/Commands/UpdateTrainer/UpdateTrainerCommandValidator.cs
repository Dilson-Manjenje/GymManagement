using FluentValidation;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Trainers.Shared;

namespace GymManagement.Application.Trainers.Commands.UpdateTrainer;

public class UpdateTrainerCommandValidator : AbstractValidator<UpdateTrainerCommand>
{

    public UpdateTrainerCommandValidator(ITrainersRepository trainersRepository)
    {
        Include(new TrainerBaseValidator());
        
        RuleFor(x => x.Id)
                .NotEmpty().WithMessage("'{PropertyName}' is required.");
    }
}
