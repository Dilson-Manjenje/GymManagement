using FluentValidation;
using GymManagement.Application.Common.Interfaces;

namespace GymManagement.Application.Members.Commands.CreateMember;

public class CreateMemberCommandValidator : AbstractValidator<CreateMemberCommand>
{
    private readonly IMembersRepository _membersRepository;

    public CreateMemberCommandValidator(IMembersRepository membersRepository)
    {
        _membersRepository = membersRepository;

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .MinimumLength(3).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
            .MaximumLength(60).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .MinimumLength(6).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
            .MaximumLength(100).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        // RuleFor(t => t.Email);

        RuleFor( c => c)
            .MustAsync(NotExistWithSameUserName)
            .WithMessage("Informed User Name already exist in the Gym.");
    }

    private async Task<bool> NotExistWithSameUserName(CreateMemberCommand command, CancellationToken token)
    {
        // HACK: Only allow create members with Gym 
        var members = await _membersRepository.ListByGymAsync(command.GymId);
        var member = members?.SingleOrDefault(m => m.UserName.Equals(command.UserName, StringComparison.InvariantCultureIgnoreCase));

        return member is null;
    }
}
