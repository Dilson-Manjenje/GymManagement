using ErrorOr;

namespace GymManagement.Domain.Trainers;

public static class TrainerErrors
{
    public static Error TrainerNotFound(Guid id) => Error.NotFound
    (code: "Trainer.NotFound",
      description: $"Trainer with ID {id} not found.");

  public static Error CantRemoveTrainerWithBookedSession(Guid id) => Error.Validation
  (code: "Trainer.CantRemoveTrainerWithBookedSession",
    description: $"Can not remove Trainer with booked sessions. TrainerID: '{id}'");
      
  public static Error TrainerAlreadyAddedToGym(Guid memberId) => Error.Conflict
  (code: "Trainer.TrainerAlreadyAddedToGym",
   description: $"Trainer with Member ID {memberId} is already in the gym.");    
      

}
