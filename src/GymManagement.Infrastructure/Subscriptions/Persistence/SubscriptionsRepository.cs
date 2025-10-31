using GymManagement.Application.Common.Interfaces;
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
                         .ToListAsync(cancellationToken);
                        
    }

    async Task ISubscriptionsRepository.AddSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        await _dbContext.Subscriptions.AddAsync(subscription, cancellationToken);
    }

    async Task<Subscription?> ISubscriptionsRepository.GetByIdAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        //return await _dbContext.Subscriptions.FirstOrDefaultAsync(subscription => subscription.Id == subscriptionId, cancellationToken);
        return await _dbContext.Subscriptions.FindAsync(subscriptionId, cancellationToken);
    }

    async Task ISubscriptionsRepository.RemoveSubscription(Subscription subscription, CancellationToken cancellationToken)
    {
        await Task.FromResult(_dbContext.Subscriptions.Remove(subscription));        
    }

    async Task ISubscriptionsRepository.UpdateAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        _dbContext.Update(subscription);

        await Task.CompletedTask;
    }

    async Task<bool> ISubscriptionsRepository.ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Subscriptions
            .AsNoTracking()
            .AnyAsync(subscription => subscription.Id == id);
    }

}