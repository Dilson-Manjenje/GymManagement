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
    }
}
