namespace GymManagement.Contracts.Subscriptions;

public record CreateSubscriptionRequest(SubstriptionType SubscriptionType, Guid AdminId);
