namespace GymManagement.Contracts.Subscriptions;

public record SubscriptionResponse(Guid Id,
                                   SubstriptionType SubscriptionType,
                                   decimal Price,
                                   string Gym,
                                   int MaxRooms,
                                   List<string> Rooms,
                                   int MaxDailySessions,
                                   Guid MemberId,
                                   string UserName
                                   );