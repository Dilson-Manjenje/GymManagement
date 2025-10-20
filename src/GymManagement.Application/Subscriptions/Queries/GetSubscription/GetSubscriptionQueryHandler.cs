using GymManagement.Domain.Subscriptions;
using MediatR;

namespace GymManagement.Application.Subscriptions.Queries.GetSubscription;

public class GetSubscriptionQueryHandler : IRequestHandler<GetSubscriptionQuery, Subscription>
{
    public async Task<Subscription> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implemente data retrieval logic here
        return await Task.FromResult(new Subscription
        {
            Id = request.Id,
            SubscriptionType = string.Empty,
            //SubscriptionType = request.SubscriptionType,
        });
    }
}