using ErrorOr;

namespace GymManagement.Domain.Members;

public static class MemberErrors
{
  public static Error UserNotFound(Guid memberId) => Error.NotFound
    (code: "User.NotFound",
      description: $"User with Member ID {memberId} not found.");

  public static Error UserDontHaveGym(string userName, Guid? memberId) => Error.Validation
    (code: "User.UserDontHaveGym",
      description: $"User {userName} with Member ID {memberId} is not associate to a Gym.");

  public static Error CannotRemoveUserWithScheduledSessions() => Error.Validation
  (code: "User.CannotRemoveUserWithScheduledSessions",
   description: $"Can not remove User with scheduled sessions.");

  public static Error UserAlreadyAddedToSession(Guid memberId) => Error.Conflict
  (code: "User.UserAlreadyAddedToSession",
   description: $"User with Member ID {memberId} already added to the session.");

  public static Error UserAlreadyHaveActiveSubscription(Guid memberId) => Error.Conflict
     (code: "User.UserAlreadyHaveActiveSubscription",
      description: $"User with Member ID {memberId} already has active subscription.");

  public static Error UserNotAssociateWithSubscription(Guid subscriptionId, Guid memberId) => Error.Validation
    (code: "User.UserNotAssociateWithSubscription",
     description: $"Subscription {subscriptionId} is not associate with the user Member ID {memberId}.");

}