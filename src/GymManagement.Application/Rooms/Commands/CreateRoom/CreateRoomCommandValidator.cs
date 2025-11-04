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
            .MustAsync(NotExistWithSame)
            .WithMessage("A room with the same name already exists.");
    }

    private async Task<bool> NotExistWithSame(CreateRoomCommand command, CancellationToken token)
    {
        var room = await _roomsRepository.GetByName(command.Name);

        return room is null;
    }
}
