using ErrorOr;

namespace GymManagement.Domain.Admins;

public static class AdminErrors
{
  public static Error UserNotFound(Guid id) => Error.NotFound
    (code: "User.NotFound",
      description: $"User with Admin ID {id} not found.");

  public static Error UserDontHaveGym(string userName, Guid? id) => Error.Validation
    (code: "User.UserDontHaveGym",
      description: $"User {userName} with Admin ID {id} is not associate to a Gym.");
      
  public static Error CannotRemoveUserWithScheduledSessions(Guid id) => Error.Validation
  (code: "User.CannotRemoveUserWithScheduledSessions",
   description: $"Can not remove User ID {id} with scheduled sessions.");

    public static Error UserAlreadyAddedToSession(Guid id) => Error.Conflict
    (code: "User.UserAlreadyAddedToSession",
     description: $"User {id} already added to the session.");    
  
}