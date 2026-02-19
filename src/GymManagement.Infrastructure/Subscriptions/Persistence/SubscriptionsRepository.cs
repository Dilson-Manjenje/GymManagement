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

    async Task<IEnumerable<Subscription>?> ISubscriptionsRepository.ListByGymAsync(Guid GymId, CancellationToken cancellationToken)
    {
        await _dbContext.Subscriptions.AddAsync(subscription, cancellationToken);
    }
    
    async Task ISubscriptionsRepository.UpdateAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        _dbContext.Subscriptions.Update(subscription);

        await Task.CompletedTask;
    }

    async Task<bool> ISubscriptionsRepository.ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        await Task.FromResult(_dbContext.Subscriptions.Remove(subscription));
    }

    async Task<Subscription?> ISubscriptionsRepository.GetByIdAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.Subscriptions
                                            .Include(s => s.Member)
                                            .Include(s => s.Member.Gym)
                                            .Include(s => s.SubscriptionRooms)
                                                .ThenInclude(sr => sr.Room)              
                                            .AsSingleQuery()
                                            .SingleOrDefaultAsync(s => s.Id == subscriptionId, cancellationToken);

        return subscription;                    
    }

    async Task<IEnumerable<Subscription>?> ISubscriptionsRepository.ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Subscriptions
                         .AsNoTracking()
                         .OrderByDescending(s => s.StartDate)
                         .Include(s => s.Member)
                         .Include(s => s.Member.Gym)
                         .Include(s => s.SubscriptionRooms)
                            .ThenInclude(sr => sr.Room)              
                         .AsSingleQuery()                                       
                         .ToListAsync(cancellationToken);

    }

    async Task<IEnumerable<Subscription>?> ISubscriptionsRepository.ListByGymAsync(Guid GymId, CancellationToken cancellationToken)
    {
        var subscriptions = await _dbContext.Subscriptions.Where(s => s.Member.GymId == GymId)
                                                    .AsNoTracking()
                                                    .OrderByDescending(s => s.StartDate)
                                                    .Include(s => s.Member)
                                                    .Include(s => s.Member.Gym)
                                                    .Include(s => s.SubscriptionRooms)
                                                        .ThenInclude(sr => sr.Room)              
                                                    .AsSingleQuery()       
                                                    .ToListAsync(cancellationToken);
        return subscriptions;
    }
    
    async Task<IEnumerable<Subscription>?> ISubscriptionsRepository.ListByMemberAsync(Guid memberId, CancellationToken cancellationToken)
    {
        var subscriptions = await _dbContext.Subscriptions.Where(s => s.MemberId == memberId)
                                                    .AsNoTracking()
                                                    .OrderByDescending(s => s.StartDate)
                                                    .Include(s => s.Member)
                                                    .Include(s => s.Member.Gym)
                                                    .Include(s => s.SubscriptionRooms)
                                                        .ThenInclude(sr => sr.Room)              
                                                    .AsSingleQuery()       
                                                    .ToListAsync(cancellationToken);
        return subscriptions;
    }

    async Task<bool> ISubscriptionsRepository.ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Subscriptions
            .AsNoTracking()
            .AnyAsync(subscription => subscription.Id == id);
    }
    
    async Task ISubscriptionsRepository.AddRoomToSubscriptionAsync(SubscriptionRooms subscRoom, CancellationToken cancellationToken)
    {
        await _dbContext.SubscriptionRooms.AddAsync(subscRoom);
    }

    async Task ISubscriptionsRepository.RemoveRoomFromSubscriptionAsync(SubscriptionRooms subscRoom, CancellationToken cancellationToken)
    {
        await Task.FromResult(_dbContext.SubscriptionRooms.Remove(subscRoom));  
    }
    
    async Task<IEnumerable<Room>> ISubscriptionsRepository.ListSubscriptionRooms(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var rooms = await _dbContext.Subscriptions
            .Where(s => s.Id == subscriptionId)
            .SelectMany(s => s.SubscriptionRooms.Select(sr => sr.Room))            
            .ToListAsync();
            
        return rooms;
    }

    async Task<IEnumerable<Subscription>> ISubscriptionsRepository.ListSubscriptionsByRoomAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var subscriptionsInTheRoom = await _dbContext.SubscriptionRooms
            .Where(x => x.RoomId == roomId)
            .Include(x => x.Subscription.Member)
            .Include(x => x.Subscription.Member.Gym)
            .Include(x => x.Room)
            .AsSingleQuery()           
            .Select(x => x.Subscription)
            .ToListAsync();

        return subscriptionsInTheRoom;
    }

    async Task<bool> ISubscriptionsRepository.HasActiveSubscription(Guid memberId, CancellationToken cancellationToken)
    {
        return await _dbContext.Subscriptions
                               .AnyAsync(s => s.MemberId == memberId
                               && s.EndDate > DateTime.Now);
                
    }
    
    
    async Task<Subscription?> ISubscriptionsRepository.GetActiveSubscriptionAsync(Guid memberId, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.Subscriptions
                                            .Where(s => s.MemberId == memberId
                                                       && s.EndDate > DateTime.Now )
                                            .Include(s => s.Member)
                                            .Include(s => s.Member.Gym)
                                            .Include(s => s.SubscriptionRooms)
                                                .ThenInclude(sr => sr.Room)              
                                            .AsSingleQuery()
                                            .SingleOrDefaultAsync(cancellationToken);

        return subscription;                    
    }

}