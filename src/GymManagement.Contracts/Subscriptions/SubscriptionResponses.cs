using GymManagement.Contracts.Common;

namespace GymManagement.Contracts.Subscriptions;

public record SubscriptionResponse(Guid Id,
                                   SubstriptionType SubscriptionType,
                                   decimal Price,
                                   DateTime StartDate,
                                   DateTime EndDate,
                                   bool IsActive,
                                   string GymName,
                                   int MaxRooms,
                                   List<string>? Rooms,
                                   int MaxDailySessions,
                                   Guid MemberId,
                                   // string MemberName,
                                   string UserName);

public record ListSubscriptionsResponse(IEnumerable<SubscriptionResponse> Data): ListResponse<SubscriptionResponse>(Data: Data);                       