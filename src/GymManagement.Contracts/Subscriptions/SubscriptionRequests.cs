namespace GymManagement.Contracts.Subscriptions;

public record CreateSubscriptionRequest(SubstriptionType SubscriptionType, Guid MemberId);

public record UpdateSubscriptionRequest(Guid Id, SubstriptionType SubscriptionType);

public record RoomSubscriptionRequest(Guid SubscriptionId, Guid RoomId);