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
                .WithMessage($"Name is already used by other Room.");
	}

	private async Task<bool> NameNotExisteForOther(UpdateRoomCommand command, CancellationToken token)
	{
		var room = await _roomsRepository.GetByName(command.Name);

		if (room is null || room.Id == command.Id) 
			return true; 

		return false;
	}
}                               