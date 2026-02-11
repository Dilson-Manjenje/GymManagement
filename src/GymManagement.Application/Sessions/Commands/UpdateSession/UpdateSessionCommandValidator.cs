using FluentValidation;
using GymManagement.Application.Sessions.Shared;

namespace GymManagement.Application.Sessions.Commands.UpdateSession;

public class UpdateSessionCommandValidator : AbstractValidator<UpdateSessionCommand>
{
    public UpdateSessionCommandValidator()
    {
        Include(new SessionBaseValidator());

        RuleFor(x => x.Id)
                .NotEmpty().WithMessage("'{PropertyName}' is required.");
    }
}