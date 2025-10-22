using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Subscriptions;
using GymManagement.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Subscriptions.Persistence;

public class SubscriptionsRepository : ISubscriptionsRepository
{
    private readonly GymManagementDbContext _dbContext;

    public SubscriptionsRepository(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    async Task ISubscriptionsRepository.AddSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        await _dbContext.Subscriptions.AddAsync(subscription);
    }

    async Task<Subscription?> ISubscriptionsRepository.GetByIdAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        return await _dbContext.Subscriptions.FindAsync(subscriptionId);
    }
}