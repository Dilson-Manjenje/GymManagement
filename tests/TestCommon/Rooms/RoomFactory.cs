using GymManagement.Domain.Rooms;
using TestCommon.TestConstants;

namespace TestCommon.Rooms;

public static class RoomFactory
{
    public static Room CreateRoom(string name = Constants.Rooms.Name,
                                  Guid? gymId = null,
                                  int capacity = 3)
    {
        return new Room(
            name: name,
            capacity: capacity,
            gymId: gymId ?? Constants.Gyms.Id);
    }
}