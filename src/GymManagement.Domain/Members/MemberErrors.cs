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

  public static Error CannotRemoveMemberWithBooking(Guid memberId) => Error.Validation
  (code: "Member.CannotRemoveMemberWithBooking",
   description: $"Can not remove Member with bookings or sessions.");

  public static Error CannotDeleteMemberWithSubscription(Guid memberId) => Error.Validation
   (code: "Member.CannotDeleteMemberWithSubscription",
    description: $"Can not delete Member with subscription.");
      
  public static Error MemberAlreadyHaveActiveSubscription(Guid memberId) => Error.Conflict
     (code: "Member.MemberAlreadyHaveActiveSubscription",
      description: $"User with Member ID {memberId} already has active subscription.");
}