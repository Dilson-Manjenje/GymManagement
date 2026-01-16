using ErrorOr;

namespace GymManagement.Domain.Members;

public static class MemberErrors
{
  public static Error MemberNotFound(Guid memberId) => Error.NotFound
    (code: "Member.NotFound",
      description: $"Member with ID {memberId} not found.");

  public static Error MemberDontHaveGym(Guid? memberId, string? userName = null) => Error.Validation
    (code: "Member.MemberDontHaveGym",
      description: $"User {userName} with Member ID {memberId} is not associate to a Gym.");

  public static Error CannotRemoveMemberWithScheduledSessions() => Error.Validation
  (code: "Member.CannotRemoveMemberWithScheduledSessions",
   description: $"Can not remove Member with scheduled sessions.");

  public static Error MemberAlreadyAddedToSession(Guid memberId) => Error.Conflict
  (code: "Member.MemberAlreadyAddedToSession",
   description: $"User with Member ID {memberId} already added to the session.");

  public static Error MemberAlreadyHaveActiveSubscription(Guid memberId) => Error.Conflict
     (code: "Member.MemberAlreadyHaveActiveSubscription",
      description: $"User with Member ID {memberId} already has active subscription.");

  public static Error MemberNotAssociateWithSubscription(Guid subscriptionId, Guid memberId) => Error.Validation
    (code: "Member.MemberNotAssociateWithSubscription",
     description: $"Subscription {subscriptionId} is not associate with the Member ID {memberId}.");

}