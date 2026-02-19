using ErrorOr;

namespace GymManagement.Domain.Rooms;

public static class RoomErrors
{
    public static readonly Error CannotExceedRoomCapacity = Error.Validation(
        "Room.CannotExceedRoomCapacity",
        "Cant exceed room capacity.");

    public static Error RoomNotFound(Guid id) => Error.NotFound
    (code: "Room.NotFound",
      description: $"Room with ID {id} not found.");

  public static Error CannotDisableRoomWithSessions(Guid id) => Error.Validation
  (code: "Room.CannotDisableRoomWithSessions",
    description: $"Can not disable Room ID {id} with scheduled sessions.");
      
  public static Error RoomHasOverlappingSession() => Error.Validation
  (code: "Room.RoomHasOverlappingSession",
    description: $"The Room already has schedule session on informed dates.");
  
  

}