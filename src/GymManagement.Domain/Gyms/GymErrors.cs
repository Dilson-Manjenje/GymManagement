using ErrorOr;

namespace GymManagement.Domain.Gyms;

public static class GymErrors
{
  public static Error GymNotFound(Guid id) => Error.NotFound
  (code: "Gym.NotFound",
    description: $"Gym with ID {id} not found.");
      
    public static Error CannotDeleteGymWithRooms(Guid id) => Error.Validation
    (code: "Gym.CannotDeleteGymWithRooms",
    description: $"Can not delete Gym {id} with associated Rooms.");
}