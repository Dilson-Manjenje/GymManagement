using ErrorOr;
using FluentValidation;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Members;
using GymManagement.Domain.Trainers;

namespace GymManagement.Application.Trainers.Commands.CreateTrainer;

public class CreateTrainerCommandValidator : AbstractValidator<CreateTrainerCommand>
{
    private readonly ITrainersRepository _trainersRepository;
    private readonly IMembersRepository _membersRepository;

    public CreateTrainerCommandValidator(ITrainersRepository trainersRepository,
                                         IMembersRepository membersRepository)
    {
        _trainersRepository = trainersRepository;
        _membersRepository = membersRepository;

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

        RuleFor(c => c)
            .MustAsync(NotExistWithSameEmail)
            .WithMessage("Already exist Trainer with informed Email Address.");
        _membersRepository = membersRepository;
    }

    private async Task<bool> NotExistWithSamePhone(CreateTrainerCommand command, CancellationToken token)
    {
        var member = await _membersRepository.GetByIdAsync(command.MemberId);
        //if (member is null) return false;

        if (member is not null && member.GymId.HasValue)
        {
            var trainers = await _trainersRepository.ListByGymIdAsync(member.GymId.Value);
            var trainerWithPhone = trainers?.SingleOrDefault(t => t.Phone.ToLower() == command.Phone);

            return trainerWithPhone is null;
        }
        
        return false;
    }

    private async Task<bool> NotExistWithSameEmail(CreateTrainerCommand command, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
            return true; 
            
        var member = await _membersRepository.GetByIdAsync(command.MemberId);

        if (member is not null && member.GymId.HasValue)
        {
            var trainers = await _trainersRepository.ListByGymIdAsync(member.GymId.Value);
            var trainerWithEmail = trainers?.SingleOrDefault(t => t.Email?.ToLower() == command.Email);

            return trainerWithEmail is null;
        }

        // throw new InvalidOperationException("Member Not Found");
        
        return false;
    }
}
