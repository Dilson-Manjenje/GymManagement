using FluentValidation;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Application.Rooms.Commands.Shared;

namespace GymManagement.Application.Rooms.Commands.CreateRoom;

public class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    private readonly IRoomsRepository _roomsRepository;

    public CreateRoomCommandValidator(IRoomsRepository roomsRepository)
    {
        _roomsRepository = roomsRepository;

       Include(new RoomBaseCommandValidator());

        RuleFor( c => c)
            .MustAsync(NotExistWithSameName)
            .WithMessage("A room with the same name already exists in this Gym.");
    }

    private async Task<bool> NotExistWithSameName(CreateRoomCommand command, CancellationToken token)
    {
        var rooms = await _roomsRepository.ListAsync();
        var room = rooms?.SingleOrDefault(r => r.Name.Equals(command.Name, StringComparison.InvariantCultureIgnoreCase)
                                            && r.GymId == command.GymId );
        
        return room is null;
    }
}
