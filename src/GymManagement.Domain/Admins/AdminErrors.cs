using ErrorOr;

namespace GymManagement.Domain.Admins;

public static class AdminErrors
{
  public static Error UserNotFound(Guid adminId) => Error.NotFound
    (code: "User.NotFound",
      description: $"User with Admin ID {adminId} not found.");

  public static Error UserDontHaveGym(string userName, Guid? adminId) => Error.Validation
    (code: "User.UserDontHaveGym",
      description: $"User {userName} with Admin ID {adminId} is not associate to a Gym.");
      
  public static Error CannotRemoveUserWithScheduledSessions() => Error.Validation
  (code: "User.CannotRemoveUserWithScheduledSessions",
   description: $"Can not remove User with scheduled sessions.");

  public static Error UserAlreadyAddedToSession(Guid adminId) => Error.Conflict
  (code: "User.UserAlreadyAddedToSession",
   description: $"User with Admin ID {adminId} already added to the session.");

  public static Error UserAlreadyHaveActiveSubscription(Guid adminId) => Error.Conflict
     (code: "User.UserAlreadyHaveActiveSubscription",
      description: $"User with Admin ID {adminId} already has active subscription."); 

  public static Error UserNotAssociateWithSubscription(Guid subscriptionId, Guid adminId) => Error.Validation
    (code: "User.UserNotAssociateWithSubscription",
     description: $"Subscription {subscriptionId} is not associate with the user Admin ID {adminId}.");        
  
}