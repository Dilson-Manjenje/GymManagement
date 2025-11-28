using FluentValidation;
using GymManagement.Application.Common.Interfaces;

namespace GymManagement.Application.Gyms.Commands.CreateGym;


public class CreateGymCommandValidator : AbstractValidator<CreateGymCommand>
{
    private readonly IGymsRepository _gymsRepository;

    public CreateGymCommandValidator(IGymsRepository gymRepository)
    {
        _gymsRepository = gymRepository;

        RuleFor(g => g.Name)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .MinimumLength(3).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
            .MaximumLength(60).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        RuleFor(g => g.Address)
            .NotEmpty().WithMessage("'{PropertyName}' é obrigatório.")
            .MaximumLength(100).WithMessage("'{PropertyName}' must have '{MaxLength}' characteres maximum.");

        RuleFor(g => g)
            .MustAsync(NotExistWithSame) // true pass false error
            .WithMessage($"Already exist Gym with this name.");
    }

    private async Task<bool> NotExistWithSame(CreateGymCommand command, CancellationToken token)
    {
        var gyms = await _gymsRepository.ListAsync();
        var gym = gyms?.SingleOrDefault(g => g.Name.Equals(command.Name, StringComparison.InvariantCultureIgnoreCase));

        return gym is null;
    }
}
