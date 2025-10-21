using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Application.Common.Interfaces;
using GymManagement.Domain.Subscriptions;

namespace GymManagement.Infrastructure.Subscriptions;

public class SubscriptionsRepository : ISubscriptionsRepository
{
    private readonly List<Subscription> _subscriptions = new();

    async Task ISubscriptionsRepository.AddSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        _subscriptions.Add(subscription);
        await Task.CompletedTask;
    }

    async Task<Subscription?> ISubscriptionsRepository.GetSubscriptionById(Guid id, CancellationToken cancellationToken)
    {
        var subscription = _subscriptions.FirstOrDefault(s => s.Id == id);
        return await Task.FromResult(subscription);
    }
}