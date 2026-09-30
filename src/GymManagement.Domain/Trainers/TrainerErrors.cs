using ErrorOr;

namespace GymManagement.Domain.Trainers;

public static class TrainerErrors
{
  public static Error TrainerNotFound(Guid id) => Error.NotFound
  (code: "Trainer.NotFound",
    description: $"Trainer with ID {id} not found.");

  public static Error CantRemoveTrainerWithSession(Guid id) => Error.Validation
  (code: "Trainer.CantRemoveTrainerWithSession",
    description: $"Can not remove Trainer with sessions. TrainerID: '{id}'");

  public static Error TrainerAlreadyAddedToGym(Guid memberId) => Error.Conflict
  (code: "Trainer.TrainerAlreadyAddedToGym",
   description: $"Trainer with Member ID {memberId} is already in the gym.");

  public static Error TrainerPhoneAlreadyExists(string phone) => Error.Conflict
  (code: "Trainer.TrainerPhoneAlreadyExists",
   description: $"Already exist Trainer with the phone number {phone}.");

  public static Error TrainerEmailAlreadyExists(string email) => Error.Conflict
  (code: "Trainer.TrainerEmailAlreadyExists",
   description: $"Already exist Trainer with the email {email}.");

}
