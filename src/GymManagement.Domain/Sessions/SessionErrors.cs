using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;

namespace GymManagement.Domain.Sessions;

public static class SessionErrors
{
  public static readonly Error CannotExceedSessionCapacity = Error.Validation(
        "Session.CannotExceedSessionCapacity",
        "The Session is full. Cant exceed the Session capacity.");

  public static Error SessionNotFound(Guid id) => Error.NotFound
    (code: "Session.SessionNotFound",
      description: $"Session with ID {id} not found.");
  public static readonly Error CannotCreateAfterBusinessHours = Error.Validation(
        "Session.CannotCreateAfterBusinessHours",
        "Session must start at or before 21:00.");

  public static Error TrainerNotInTheSameGym(Guid trainerId) => Error.NotFound
  (code: "Session.TrainerNotInTheSameGym",
    description: $"The Trainer with ID {trainerId} was not found in the Gym of the Room.");

  public static Error CantCancelSessionWithBooking(Guid id) => Error.Validation
  (code: "Session.CantCancelSessionWithBooking",
      description: $"Can not cancel/delete Session {id} with active Booking.");

  public static Error CantChangeSession(Guid id) => Error.Validation
  (code: "Session.CantChangeSession",
      description: $"Cant change Session {id} with its current status.");

  public static Error CantChangeGym() => Error.Validation
  (code: "Session.CantChangeGym",
      description: $"Cant change Gym of the Session, selected a Room from same Gym.");
  
      
}