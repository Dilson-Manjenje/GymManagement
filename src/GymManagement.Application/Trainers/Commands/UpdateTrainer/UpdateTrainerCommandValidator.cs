using FluentValidation;
using GymManagement.Application.Common.Interfaces;

namespace GymManagement.Application.Trainers.Commands.UpdateTrainer;

public class UpdateTrainerCommandValidator : AbstractValidator<UpdateTrainerCommand>
{
    private readonly ITrainersRepository _trainersRepository;

    public UpdateTrainerCommandValidator(ITrainersRepository trainersRepository)
    {
        _trainersRepository = trainersRepository;

        RuleFor(t => t.Name)
            .MinimumLength(3).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
            .MaximumLength(60).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        RuleFor(t => t.Phone)
           .MinimumLength(9).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
           .MaximumLength(15).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        RuleFor(t => t.Email)
                .EmailAddress().WithMessage("A valid email is required")
                .MaximumLength(100).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");
                
        //RuleFor(x => x.Email).EmailAddress(EmailValidationMode.Net4xRegex);

        RuleFor(t => t.Specialization)
           .MinimumLength(3).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
           .MaximumLength(100).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");
            
        RuleFor(c => c)
            .MustAsync(NotExistWithSamePhone) // true pass false error
            .WithMessage("Already exist Trainer with this Phone Number.");
            
        RuleFor( c => c)
            .MustAsync(NotExistWithSameEmail)
            .WithMessage("Already exist Trainer with informed Email Address.");
    }

    private async Task<bool> NotExistWithSamePhone(UpdateTrainerCommand command, CancellationToken token)
    {
        var trainer = await _trainersRepository.GetByIdAsync(command.Id);        
        if (trainer is null)
            return false; 

        var trainers = await _trainersRepository.ListByGymIdAsync(trainer.GymId);
        var trainerWithPhone = trainers?.SingleOrDefault(t => t.Phone.ToLower() == command.Phone);


        return trainerWithPhone is null || trainerWithPhone.Id == command.Id;
    }

    private async Task<bool> NotExistWithSameEmail(UpdateTrainerCommand command, CancellationToken token)
    {
        if (string.IsNullOrEmpty(command.Email))
            return true;
            
        var trainer = await _trainersRepository.GetByIdAsync(command.Id);
        if (trainer is null)
            return false; 

        var trainers = await _trainersRepository.ListByGymIdAsync(trainer.GymId);
        var trainerWithEmail = trainers?.SingleOrDefault(t => t.Email?.ToLower() == command.Email);

        return trainerWithEmail is null || trainerWithEmail.Id == command.Id;
    }
}
