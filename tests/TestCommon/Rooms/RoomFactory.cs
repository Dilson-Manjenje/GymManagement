using GymManagement.Domain.Rooms;
using TestCommon.TestConstants;

namespace TestCommon.Rooms;

public static class RoomFactory
{
    public static Room CreateRoom(string? name = null,
                                  Guid? gymId = null,
                                  int capacity = 3)
    {
        return new Room(
            name: name ?? Constants.Rooms.Name,
            capacity: capacity,
            gymId: gymId ?? Constants.Gyms.NewId);
    }

    public static Room GetKickBoxigRoomOfFightGym()
    {
        return new Room(
            id: Constants.Rooms.KickBoxingRoomId,
            name: "Kick-Boxing",
            capacity: 3,
            gymId: Constants.Gyms.FightGymId);
    }
}
