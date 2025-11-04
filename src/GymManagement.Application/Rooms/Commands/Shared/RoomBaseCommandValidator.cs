using FluentValidation;
using GymManagement.Application.Common.Interfaces;

namespace GymManagement.Application.Rooms.Commands.Shared;

public class RoomBaseCommandValidator : AbstractValidator<RoomBaseCommand>
{
    //private readonly IRoomsRepository _roomsRepository;

    public RoomBaseCommandValidator()
    {
        //_roomsRepository = roomsRepository;

        RuleFor( r => r.Name)
            .NotEmpty().WithMessage("'{PropertyName}' is required.")
            .MinimumLength(3).WithMessage("'{PropertyName}' must have at least {MinLength} characters.")
            .MaximumLength(100).WithMessage("'{PropertyName}' must have at most {MaxLength} characters.");

        RuleFor( r => r.Capacity)
            .GreaterThanOrEqualTo(1).WithMessage("'{PropertyName}' must be greater than 1.");
    }
}
