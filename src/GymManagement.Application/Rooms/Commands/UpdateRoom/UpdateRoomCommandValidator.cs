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
                 .NotEmpty().WithMessage("'{PropertyName}' é obrigatório.");
     
        RuleFor( r => r)
                .MustAsync(NameNotExisteForOther) 
                .WithMessage($"Name already used by other Room in this Gym.");
	}

	private async Task<bool> NameNotExisteForOther(UpdateRoomCommand command, CancellationToken token)
	{
		var rooms = await _roomsRepository.ListAsync();
		var room = rooms?.SingleOrDefault(r => r.Name.Equals(command.Name, StringComparison.InvariantCultureIgnoreCase)
		 									&& r.GymId == command.GymId);

		if (room is null || room.Id == command.Id) 
			return true; 

		return false;
	}
}                               