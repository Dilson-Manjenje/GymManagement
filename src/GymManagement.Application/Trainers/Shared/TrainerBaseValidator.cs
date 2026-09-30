using FluentValidation;

namespace GymManagement.Application.Trainers.Shared;

public class TrainerBaseValidator : AbstractValidator<TrainerBaseCommand>
{
    public TrainerBaseValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .MinimumLength(3).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
            .MaximumLength(60).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        RuleFor(t => t.Phone)
           .NotEmpty().WithMessage("'{PropertyName}' is required.")
           //.Matches(@"^\d{9}$").WithMessage("'{PropertyName}' must have 9 digits.")
           .Must(x => !string.IsNullOrEmpty(x) && x.All(char.IsDigit)).WithMessage("'{PropertyName}' must have 9 digits.")
           .MinimumLength(9).WithMessage("'{PropertyName}' must have at least {MinLength} digits.")
           .MaximumLength(12).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        RuleFor(t => t.Email)
                //.NotEmpty().WithMessage("Email address is required")  
                .EmailAddress().WithMessage("A valid email is required")
                .MaximumLength(100).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        //RuleFor(x => x.Email).EmailAddress(EmailValidationMode.Net4xRegex);

        RuleFor(t => t.Specialization)
           .NotEmpty().WithMessage("'{PropertyName}' is required.")
           .MinimumLength(3).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
           .MaximumLength(100).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");
    }
}
