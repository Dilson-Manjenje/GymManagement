using ErrorOr; 

namespace GymManagement.Domain.Subscriptions;

public static class SubscriptionErrors
{
  public static Error SubscriptionNotFound(Guid id) => Error.NotFound
  (code: "Subscription.NotFound",
    description: $"Subscription with ID {id} not found.");

  public static Error InvalidSubscriptionType(string name) => Error.Validation
  (code: "Subscription.InvalidSubscriptionType",
  description: $"SubscriptionType with name {name} is invalid.");
  public static Error SubscriptionAlreadyExists() => Error.Conflict
  (code: "Subscription.AlreadyExists",
    description: $"Subscription with given details already exists.");

  public static Error HasMaxRoomsAllowed() => Error.Validation
  (code: "Subscription.HasMaxRoomsAllowed",
  description: $"Subscription has the maximum of rooms allowed.");
    
  public static Error RoomAlreadyAssociated(Guid roomId) => Error.Conflict
  (code: "Subscription.RoomAlreadyAssociated",
    description: $"The Room with ID {roomId} is already associated to the Subscription.");

  public static Error RoomWasNotFoundInMemberGym(Guid roomId) => Error.NotFound
  (code: "Subscription.RoomWasNotFoundInMemberGym",
    description: $"The Room with ID {roomId} was not found on member's gym.");
  
   public static Error RoomNotInSubscription(Guid roomId) => Error.NotFound
  (code: "Subscription.RoomNotInSubscription",
    description: $"The Room with ID {roomId} was not found in the Subscription.");
}