using FluentValidation;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Rooms.Commands.Shared;

namespace GymManagement.Application.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
	private readonly IRoomsRepository _roomsRepository;
    public UpdateRoomCommandValidator(IRoomsRepository roomsRepository)
    {
        _roomsRepository = roomsRepository;

		Include(new RoomBaseCommandValidator());
		
        RuleFor(r => r.Id)
                 .NotEmpty().WithMessage("'{PropertyName}' is required.");
	}
}                               