using ErrorOr;

namespace GymManagement.Domain.Trainers;

public static class TrainerErrors
{
    public static Error TrainerNotFound(Guid id) => Error.NotFound
    (code: "Trainer.NotFound",
      description: $"Trainer with ID {id} not found.");

  public static Error CannotRemoveTrainerWithSessions(string name) => Error.Validation
  (code: "Trainer.CannotRemoveTrainerWithSessions",
    description: $"Can not remove Trainer {name} with scheduled sessions.");
      
  public static Error TrainerAlreadyAddedToGym(Guid id) => Error.Conflict
  (code: "Trainer.TrainerAlreadyAddedToGym",
   description: $"Trainer {id} already added to the gym.");    
      

}
