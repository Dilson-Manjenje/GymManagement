using FluentValidation;

namespace GymManagement.Application.Sessions.Shared;

public class SessionBaseValidator : AbstractValidator<SessionBaseCommand>
{
    public SessionBaseValidator()
    {
        // TODO: Inject IDateTimeProvider for clock 
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .MinimumLength(6).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
            .MaximumLength(60).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.Now)
                .When(x => x.StartDate != null, ApplyConditionTo.CurrentValidator)
                .WithMessage("'{PropertyName}' must be greater than now.")            
            .LessThan(x => x.EndDate)
                .When(x => x.StartDate != null && x.EndDate != null, ApplyConditionTo.CurrentValidator)
                .WithMessage("'{PropertyName}' must be less than {ComparisonProperty}.")
            .Must(date => date == null || date.Value.TimeOfDay <= TimeSpan.FromHours(22)) // 22 is a local business rule, not UTC
                .WithMessage("Session must start at or before 22:00.");
    }
}
