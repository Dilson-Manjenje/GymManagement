using GymManagement.Application.Rooms.Commands.CreateRoom;

namespace TestCommon.Rooms;

public static class RoomCommandFactory
{
    public static CreateRoomCommand CreateRoomCommand(Guid gymId, string? name = null, int capacity = 0)
    {
        return new CreateRoomCommand(GymId: gymId, Name: name!, Capacity: capacity);        
    }
}