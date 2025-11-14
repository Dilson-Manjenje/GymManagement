using FluentValidation;
using GymManagement.Application.Common.Interfaces;

namespace GymManagement.Application.Gyms.Commands.UpdateGym;

public class UpdateGymCommandValidator : AbstractValidator<UpdateGymCommand>
{
	private readonly IGymsRepository _gymsRepository;
    public UpdateGymCommandValidator(IGymsRepository gymsRepository)
    {
        _gymsRepository = gymsRepository;

        RuleFor(g => g.Id)
                 .NotEmpty().WithMessage("'{PropertyName}' é obrigatório.");
        
        RuleFor(g => g.Name)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .MinimumLength(3).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
            .MaximumLength(60).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");
        
        RuleFor(g => g.Address)
            .NotEmpty().WithMessage("'{PropertyName}' é obrigatório.")
            .MaximumLength(100).WithMessage("'{PropertyName}' must have '{MaxLength}' characteres maximum.");
    
    
        RuleFor(g => g)
                .MustAsync(NameNotExisteForOther) 
                .WithMessage($"Name is already used by other gym.");
	}

	private async Task<bool> NameNotExisteForOther(UpdateGymCommand command, CancellationToken token)
	{
		var gymExistent = await _gymsRepository.GetByNameAsync(command.Name);

		if (gymExistent is null || gymExistent.Id == command.Id) 
			return true; 

		return false;
	}
}                               