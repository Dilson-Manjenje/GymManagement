using FluentValidation;
using GymManagement.Application.Common.Interfaces;

namespace GymManagement.Application.Members.Commands.UpdateMember;

public class UpdateMemberCommandValidator : AbstractValidator<UpdateMemberCommand>
{
	private readonly IMembersRepository _membersRepository;
    public UpdateMemberCommandValidator(IMembersRepository membersRepository)
    {
        _membersRepository = membersRepository;

		RuleFor(x => x.Id)
				 .NotEmpty().WithMessage("'{PropertyName}' is required.");

		RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .MinimumLength(3).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
            .MaximumLength(60).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

		RuleFor(x => x.Password)
			//.NotEmpty().WithMessage("'{PropertyName}' is required.")
			.MinimumLength(6).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
			.MaximumLength(100).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");
			
        RuleFor(x => x)
                .MustAsync(NotExistWithSameUserName) 
                .WithMessage($"User Name already used by other Member in this Gym.");
	}

	private async Task<bool> NotExistWithSameUserName(UpdateMemberCommand command, CancellationToken token)
	{
		var member = await _membersRepository.GetByIdAsync(command.Id);

		if (member is not null && member.GymId.HasValue)
		{
			var membersInTheGym = await _membersRepository.ListByGymAsync(member.GymId.Value);
			var memberWithUserName = membersInTheGym?.SingleOrDefault(m => m.UserName.Equals(command.UserName, StringComparison.InvariantCultureIgnoreCase));

			return memberWithUserName is null || memberWithUserName.Id == command.Id; // true pass, false error
		}
		
		// Update admin user via application
		var allMembers = await _membersRepository.ListAsync();
		var adminWithUserName = allMembers?.SingleOrDefault(m => m.GymId == null &&
																 m.UserName.Equals(command.UserName, StringComparison.InvariantCultureIgnoreCase));

		return adminWithUserName is null || adminWithUserName.Id == command.Id;
	}
}                               