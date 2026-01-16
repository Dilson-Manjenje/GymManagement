using GymManagement.Domain.Rooms;
using GymManagement.Domain.Subscriptions;

namespace GymManagement.Application.Common.Interfaces;

public interface ISubscriptionsRepository
{
    Task AddAsync(Subscription subscription, CancellationToken cancellationToken = default);
    Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default);
    Task RemoveAsync(Subscription subscription, CancellationToken cancellationToken = default);
    Task<Subscription?> GetByIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>?> ListAsync(CancellationToken cancellationToken = default);  
    Task<IEnumerable<Subscription>?> ListByGymAsync(Guid gymId, CancellationToken cancellationToken = default);  
    Task<IEnumerable<Subscription>?> ListByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);  
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddRoomToSubscriptionAsync(SubscriptionRooms subscRoom, CancellationToken cancellationToken = default);
    Task RemoveRoomFromSubscriptionAsync(SubscriptionRooms subscRoom, CancellationToken cancellationToken = default);
    Task<IEnumerable<Room>> ListSubscriptionRooms(Guid subscriptionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>> ListSubscriptionsByRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task<bool> HasActiveSubscription(Guid memberId, CancellationToken cancellationToken = default);
}