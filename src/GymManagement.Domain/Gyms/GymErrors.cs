using ErrorOr;

namespace GymManagement.Domain.Gyms;

public static class GymErrors
{
    public static readonly Error CantExceedMaxRooms = Error.Validation(
        "Gym.CantExceedMaxRooms",
        "A gym cant exceed maximum number of rooms.");

    public static Error GymNotFound(Guid id) => Error.NotFound
    (code: "Gym.NotFound",
      description: $"Gym with ID {id} not found.");

  public static Error CannotRemoveGymWithScheduledSessions(Guid id) => Error.Validation
  (code: "Gym.CannotRemoveGymWithScheduledSessions",
   description: $"Can not remove Gym ID {id} with Rooms with scheduled sessions.");

  public static Error RoomAlreadyAddedToGym(Guid id) => Error.Conflict
  (code: "Gym.RoomAlreadyAddedToGym",
   description: $"Room {id} already added to the gym.");    
}