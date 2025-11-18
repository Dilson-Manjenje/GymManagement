using FluentValidation;
using GymManagement.Application.Common.Interfaces;

namespace GymManagement.Application.Trainers.Commands.CreateTrainer;

public class CreateTrainerCommandValidator : AbstractValidator<CreateTrainerCommand>
{
    private readonly ITrainersRepository _trainersRepository;

    public CreateTrainerCommandValidator(ITrainersRepository trainersRepository)
    {
        _trainersRepository = trainersRepository;

        RuleFor(t => t.Name)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .MinimumLength(3).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
            .MaximumLength(60).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        RuleFor(t => t.Phone)
           .NotEmpty().WithMessage("'{PropertyName}' is required.")
           .MinimumLength(9).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
           .MaximumLength(15).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        RuleFor(t => t.Email)
                //.NotEmpty().WithMessage("Email address is required")                
                .EmailAddress().WithMessage("A valid email is required")
                .MaximumLength(100).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");
                
        //RuleFor(x => x.Email).EmailAddress(EmailValidationMode.Net4xRegex);

        RuleFor(t => t.Specialization)
           .NotEmpty().WithMessage("'{PropertyName}' is required.")
           .MinimumLength(3).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
           .MaximumLength(100).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");
            
        RuleFor(c => c)
            .MustAsync(NotExistWithSamePhone) // true pass false error
            .WithMessage("Already exist Trainer with this Phone Number.");
            
        RuleFor( c => c)
            .MustAsync(NotExistWithSameEmail)
            .WithMessage("Already exist Trainer with informed Email Address.");
    }

    private async Task<bool> NotExistWithSamePhone(CreateTrainerCommand command, CancellationToken token)
    {
        var trainers = await _trainersRepository.ListAsync();
        var trainer = trainers?.SingleOrDefault(t => t.Phone.ToLower() == command.Phone);
        
        return trainer is null;
    }

     private async Task<bool> NotExistWithSameEmail(CreateTrainerCommand command, CancellationToken token)
    {
        var trainers = await _trainersRepository.ListAsync();        
        var trainer = trainers?.SingleOrDefault( t => t.Email?.ToLower() == command.Email);

        return trainer is null;
    }
}
