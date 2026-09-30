using FluentValidation;
using GymManagement.Application.Trainers.Shared;

namespace GymManagement.Application.Trainers.Commands.CreateTrainer;

public class CreateTrainerCommandValidator : AbstractValidator<CreateTrainerCommand>
{
    public CreateTrainerCommandValidator()
    {
        Include(new TrainerBaseValidator());

        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("'{PropertyName}' is required.");
    }
}
