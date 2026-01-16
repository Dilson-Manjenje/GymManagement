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
     
        RuleFor( r => r)
                .MustAsync(NameNotExistForOther) 
                .WithMessage($"Name already used by other Room in this Gym.");
	}

	private async Task<bool> NameNotExistForOther(UpdateRoomCommand command, CancellationToken token)
	{
		var rooms = await _roomsRepository.ListByGymAsync(command.GymId);
		var room = rooms?.SingleOrDefault(r => r.Name.Equals(command.Name, StringComparison.InvariantCultureIgnoreCase));

		if (room is null || room.Id == command.Id) 
			return true; 

		return false;
	}
}                               