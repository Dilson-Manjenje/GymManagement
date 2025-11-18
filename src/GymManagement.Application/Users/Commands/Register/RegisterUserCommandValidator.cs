using FluentValidation;
using GymManagement.Application.Common.Interfaces;

namespace GymManagement.Application.Users.Commands.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IAdminsRepository _adminsRepository;

    public RegisterUserCommandValidator(IAdminsRepository adminsRepository)
    {
        _adminsRepository = adminsRepository;

        RuleFor(t => t.UserName)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .MinimumLength(3).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
            .MaximumLength(60).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        RuleFor(t => t.Password)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .MinimumLength(6).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
            .MaximumLength(100).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        // RuleFor(t => t.Email);
 
        // RuleFor( c => c)
        //     .MustAsync(NotExistWithSameEmail)
        //     .WithMessage("Already exist User with informed Email Address.");
    }

}
