using GymManagement.Domain.Rooms;
using GymManagement.Domain.Sessions;
using TestCommon.Rooms;
using TestCommon.TestConstants;

namespace TestCommon.Sessions;

public static class SessionFactory
{
    public static Session CreateSession(string title = Constants.Sessions.Title,
                                        Guid? roomId = null,
                                        Guid? trainerId = null,
                                        DateTime? startDate = null,
                                        DateTime? endDate = null,
                                        int vacancy = 1,
                                        int capacity = 1)
    {
        var session = Session.Create(roomId: roomId ?? Constants.Rooms.NewId,
                              trainerId: trainerId ?? Constants.Trainers.Id,
                              title: title,
                              capacity: Constants.Rooms.Capacity,
                              vacancy: Constants.Rooms.Capacity,
                              startDate: Constants.Sessions.StartDate,
                              endDate: Constants.Sessions.EndDate);

        session.Room = RoomFactory.CreateRoom(gymId: Constants.Gyms.NewId);
        
        return session;
    }
}