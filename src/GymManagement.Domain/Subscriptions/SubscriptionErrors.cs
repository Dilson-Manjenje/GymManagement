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
}