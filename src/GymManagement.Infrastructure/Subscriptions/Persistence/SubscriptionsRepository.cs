using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Rooms;
using GymManagement.Domain.Subscriptions;
using GymManagement.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Subscriptions.Persistence;

internal class SubscriptionsRepository : ISubscriptionsRepository
{
    private readonly GymManagementDbContext _dbContext;

    public SubscriptionsRepository(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    async Task<IEnumerable<Subscription>?> ISubscriptionsRepository.ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Subscriptions
                         .AsNoTracking()
                         .Include(s => s.Member)
                            .ThenInclude(m => m.Gym)
                         .ToListAsync(cancellationToken);

    }

    async Task ISubscriptionsRepository.AddAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        await _dbContext.Subscriptions.AddAsync(subscription, cancellationToken);
    }

    async Task<Subscription?> ISubscriptionsRepository.GetByIdAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        //return await _dbContext.Subscriptions.FindAsync(subscriptionId, cancellationToken);
        return await _dbContext.Subscriptions
                    .Include(s => s.Member)
                        .ThenInclude(m => m.Gym)
                    .Include( s => s.SubscriptionRooms )
                        .ThenInclude(sr => sr.Room)
                    .SingleOrDefaultAsync(s => s.Id == subscriptionId, cancellationToken);                    
    }

    async Task ISubscriptionsRepository.RemoveAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        await Task.FromResult(_dbContext.Subscriptions.Remove(subscription));        
    }

    async Task ISubscriptionsRepository.UpdateAsync(Subscription subscription, CancellationToken cancellationToken)
    {        
        _dbContext.Subscriptions.Update(subscription);

        await Task.CompletedTask;
    }
    async Task ISubscriptionsRepository.AddRoomToSubscriptionAsync(SubscriptionRooms subscRoom, CancellationToken cancellationToken)
    {
        await _dbContext.SubscriptionRooms.AddAsync(subscRoom);
    }

    async Task ISubscriptionsRepository.RemoveRoomFromSubscriptionAsync(SubscriptionRooms subscRoom, CancellationToken cancellationToken)
    {
        //var entity = await _dbContext.SubscriptionRooms.FindAsync(subscRoom.Id);
        await Task.FromResult(_dbContext.SubscriptionRooms.Remove(subscRoom));  
    }

    
    async Task<bool> ISubscriptionsRepository.ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Subscriptions
            .AsNoTracking()
            .AnyAsync(subscription => subscription.Id == id);
    }

    async Task<IEnumerable<Room>> ISubscriptionsRepository.GetSubscriptionRooms(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var rooms = await _dbContext.Subscriptions
            .Where(s => s.Id == subscriptionId)
            .Include(s => s.Member)
            .SelectMany(s => s.SubscriptionRooms.Select(sr => sr.Room))            
            .ToListAsync();
            
        return rooms;
    }

    async Task<IEnumerable<Subscription>> ISubscriptionsRepository.GetAllSubscriptionsByRoomAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var subscriptionsInTheRoom = await _dbContext.SubscriptionRooms
            .Where(s => s.RoomId == roomId)
            .Select(x => x.Subscription)
            .ToListAsync();

        return subscriptionsInTheRoom;          
    }
}