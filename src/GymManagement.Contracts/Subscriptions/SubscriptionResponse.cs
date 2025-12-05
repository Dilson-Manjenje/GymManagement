namespace GymManagement.Contracts.Subscriptions;

public record SubscriptionResponse(Guid Id,
                                   SubstriptionType SubscriptionType,
                                   int Price,
                                   int MaxRooms,
                                   int MaxDailySessions,
                                   Guid AdminId,
                                   string UserName 
                                   );