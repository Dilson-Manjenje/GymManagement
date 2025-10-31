namespace GymManagement.Contracts.Subscriptions;

public record UpdateSubscriptionRequest(Guid Id, SubstriptionType SubscriptionType);
